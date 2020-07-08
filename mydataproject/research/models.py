from django.db import models
from django.db.models.signals import post_save
from django.contrib.auth.models import User
from django.forms import ModelForm
import requests, json
import datetime
from aalto.models import Person
from orciddata import orcidutils

class Datasource(models.Model):
    name = models.CharField(max_length=256)

    def __str__(self):
        return self.name

class AreaOfInterest(models.Model):
    name = models.CharField(max_length=256)
    order = models.PositiveSmallIntegerField

    def __str__(self):
        return self.name

class ResearchProfile(models.Model):
    user = models.OneToOneField(User, on_delete=models.CASCADE, related_name='researchprofile')
    active = models.BooleanField(default=False)
    test_orcid_id = models.CharField(max_length=20, blank=True)
    include_orcid_id_in_profile = models.BooleanField(default=True)
    areas_of_interest = models.ManyToManyField(AreaOfInterest)
    is_aalto = models.BooleanField(default=False)

    def test_orcid_id_is_valid(self):
        return self.test_orcid_id is not None and len(self.test_orcid_id) == 19

    def get_orcid_id(self):
        if self.test_orcid_id_is_valid():
            return self.test_orcid_id 
        else:
            return self.user.username

    def get_visible_orcid_id(self):
        return self.test_orcid_id if self.test_orcid_id_is_valid() else self.user.username

    def update_or_create_publication(self, p_doi, p_datasource, p_name, p_publicationYear, p_includeInProfile):
        try:
            if p_doi is not None and len(p_doi) > 10:
                try:
                    oldPublication = Publication.objects.get(doi=p_doi, researchprofile=self)
                    oldPublication.datasources.add(p_datasource)
                    oldPublication.save()
                    return True
                except Publication.DoesNotExist:
                    pass
            newPublication = Publication.objects.create(
                researchprofile = self,
                name = p_name,
                publicationYear = p_publicationYear,
                doi = p_doi,
                includeInProfile = True
            )
            newPublication.datasources.add(p_datasource)
            newPublication.save()
        except Exception as e:
            print("Exception in orcid_record_json_to_model() works, when creating Publication object")
            print(e)
            return False

    def getLinkHtml(self, url, name):
        return '<a href="' + url + '" target="_blank">' + name + '</a>'

    def delete_orcid_data(self):
        datasource_orcid = Datasource.objects.get(name="ORCID")
        orcidutils.delete_data(self, datasource_orcid)

    def delete_ttv_data(self):
        datasource_ttv = Datasource.objects.get(name="TTV")
        # Delete publications whose only data source is TTV.
        # If there are other data sources, keep the publication but remove the TTV datasource.
        publications = Publication.objects.filter(researchprofile = self, datasources=datasource_ttv)
        for p in publications:
            if p.datasources.count() == 1:
                p.delete()
            else:
                p.datasources.remove(datasource_ttv)
        self.save()

    def delete_manual_data(self):
        datasource_manual = Datasource.objects.get(name="MANUAL")
        # Delete publications added manually.
        # If there are other data sources, keep the publication but remove the MANUAL datasource.
        publications = Publication.objects.filter(researchprofile = self, datasources=datasource_manual)
        for p in publications:
            if p.datasources.count() == 1:
                p.delete()
            else:
                p.datasources.remove(datasource_manual)
        self.save()

    def delete_all_data(self):
        self.delete_orcid_data()
        self.delete_ttv_data()
        self.delete_manual_data()
        self.delete_dummy_organization_data()
        self.delete_aalto_data()

    def get_orcid_data(self):
        datasource_orcid = Datasource.objects.get(name="ORCID")
        orcidutils.get_data(self, datasource_orcid)

    def get_virta_publications(self):
        headers = {
            'Accept': 'application/json',
        }

        # VIRTA API (json)
        url = 'https://virta-jtp.csc.fi/api/julkaisut/haku?orcid=' + self.get_orcid_id()
        print("VIRTA URL = " + url)
        response = requests.get(url, headers=headers)
        print("VIRTA http response code " + str(response.status_code))
        if response.status_code == 200:
            try:
                virta_json_data = response.json()

                # Create publication objects
                datasource_ttv = Datasource.objects.get(name="TTV")
                for obj in virta_json_data:
                    try:
                        doi = obj.get("doi", None)
                        name = obj.get("julkaisunNimi", None)
                        publicationYear = obj.get("julkaisuVuosi", 0)
                        
                        self.update_or_create_publication(doi, datasource_ttv, name, publicationYear, True)
                    except Exception as e:
                        print("Exception when creating Publication objects from Virta json")
                        print(e)
                        pass
            except Exception as e:
                print("Exception in get_virta_publications()")
                print(e)
                pass

        return True

    def add_organization_data(self):
        self.add_org1_data()
        self.add_org2_data()

    def get_all_data(self):
        self.get_orcid_data()
        self.get_virta_publications()
        self.add_organization_data()
        self.auto_include_in_profile()

    def add_org1_data(self):
        if self.user.orcid_permission.read_all_org1:
            datasource = Datasource.objects.get(name="ORG1")

            orcid_id = self.get_orcid_id()

            # Search Aalto data by ORCID ID
            print("------")
            print("Search aalto data for ORCID " + orcid_id)
            try:
                aalto_person = Person.objects.get(orcid=orcid_id)
            except Person.DoesNotExist:
                aalto_person = None

            if aalto_person is not None:
                self.add_aalto_data(aalto_person)
            else:
                # Dummy last name
                lastName = PersonLastName.objects.create(
                    researchprofile = self,
                    datasource = datasource,
                    includeInProfile = False,
                    value = 'Virtanen'
                )
                self.last_names.add(lastName)

                # Dummy first name
                firstName = PersonFirstName.objects.create(
                    researchprofile = self,
                    datasource = datasource,
                    includeInProfile = False,
                    value = 'A'
                )
                self.first_names.add(firstName)

                # Dummy other names
                otherName = PersonOtherName.objects.create(
                    researchprofile = self,
                    datasource = datasource,
                    includeInProfile = False,
                    value = 'A. Virtanen'
                )
                self.other_names.add(otherName)

                otherName2 = PersonOtherName.objects.create(
                    researchprofile = self,
                    datasource = datasource,
                    includeInProfile = False,
                    value = 'B. Virtanen'
                )
                self.other_names.add(otherName2)

                # Links
                PersonLink.objects.create(researchprofile=self, datasource=datasource, includeInProfile=False, url='https://www.google.com', name='Google')
                PersonLink.objects.create(researchprofile=self, datasource=datasource, includeInProfile=False, url='https://www.facebook.com', name='Facebook')
                PersonLink.objects.create(researchprofile=self, datasource=datasource, includeInProfile=False, url='https://www.linkedin.com', name='LinkedIn')

                # Email
                email = PersonEmail.objects.create(
                    researchprofile = self,
                    datasource = datasource,
                    includeInProfile = False,
                    value = 'abcd@example.comm'
                )
                self.emails.add(email)
            
            self.save()

    def add_org2_data(self):
        if self.user.orcid_permission.read_all_org2:
            datasource = Datasource.objects.get(name="ORG2")

            # Dummy last name
            lastName = PersonLastName.objects.create(
                researchprofile = self,
                datasource = datasource,
                includeInProfile = False,
                value = 'Anderson'
            )
            self.last_names.add(lastName)

            # Dummy first name
            firstName = PersonFirstName.objects.create(
                researchprofile = self,
                datasource = datasource,
                includeInProfile = False,
                value = 'D'
            )
            self.first_names.add(firstName)

            # Dummy other names
            otherName = PersonOtherName.objects.create(
                researchprofile = self,
                datasource = datasource,
                includeInProfile = False,
                value = 'D.A.'
            )
            self.other_names.add(otherName)

            # Links
            PersonLink.objects.create(researchprofile=self, datasource=datasource, includeInProfile=False, url='https://www.github.com', name='GitHub')
            PersonLink.objects.create(researchprofile=self, datasource=datasource, includeInProfile=False, url='https://www.research.fi', name='Research.fi')

            # Phone
            phoneObj = PersonPhone.objects.create(
                researchprofile = self,
                datasource = datasource,
                includeInProfile = False,
                value = '+358 50 222 2222'
            )
            self.phones.add(phoneObj)

            self.save()
   
    def delete_org1_data(self):
        ds = Datasource.objects.get(name="ORG1")
        self.last_names.filter(datasource=ds).delete()
        self.first_names.filter(datasource=ds).delete()
        self.other_names.filter(datasource=ds).delete()
        self.links.filter(datasource=ds).delete()
        self.emails.filter(datasource=ds).delete()
        self.phones.filter(datasource=ds).delete()

    def delete_org2_data(self):
        ds = Datasource.objects.get(name="ORG2")
        self.last_names.filter(datasource=ds).delete()
        self.first_names.filter(datasource=ds).delete()
        self.other_names.filter(datasource=ds).delete()
        self.links.filter(datasource=ds).delete()
        self.emails.filter(datasource=ds).delete()
        self.phones.filter(datasource=ds).delete()

    def delete_dummy_organization_data(self):
        self.delete_org1_data()
        self.delete_org2_data()

    def add_aalto_data(self, aalto_person):
        self.is_aalto = True
        datasource = Datasource.objects.get(name="ORG1")

        # Last name
        lastName = PersonLastName.objects.create(
            researchprofile = self,
            datasource = datasource,
            includeInProfile = False,
            value = aalto_person.last_name
        )
        self.last_names.add(lastName)

        # First name
        firstName = PersonFirstName.objects.create(
            researchprofile = self,
            datasource = datasource,
            includeInProfile = False,
            value = aalto_person.first_name
        )
        self.first_names.add(firstName)

        # Email
        if aalto_person.email is not None:
            PersonEmail.objects.create(
                researchprofile = self,
                datasource = datasource,
                includeInProfile = False,
                value = aalto_person.email
            )

        # Links
        links = []
        for link in aalto_person.links.all():
            PersonLink.objects.create(
                researchprofile = self,
                datasource = datasource,
                includeInProfile = False,
                url = link.url,
                name = link.name
            )

        # Biography
        if aalto_person.biography is not None:
            PersonBiography.objects.create(
                researchprofile = self,
                datasource = datasource,
                includeInProfile = False,
                value = aalto_person.biography
            )

        # Keywords
        keywords = []
        for keyword in aalto_person.keywords.all():
            PersonKeyword.objects.create(
                researchprofile = self,
                datasource = datasource,
                includeInProfile = False,
                value = keyword.value
            )

        # Employment
        for affiliation in aalto_person.affiliations.all():
            employmentDict = {
                'researchprofile': self,
                'datasource': datasource,
                'organizationName': 'Aalto',
                'departmentName': affiliation.department_name,
                'roleTitle': affiliation.title,
                'startYear': None,
                'startMonth': None,
                'startDay': None,
                'endYear': None,
                'endMonth': None,
                'endDay': None,
                'includeInProfile': False
            }

            try:
                employment = Employment(**employmentDict)
                employment.save()
            except Exception as e:
                print("Exception in add_aalto_data() employments")
                print(e)
                pass

        # Merit
        for m in aalto_person.merits.all():
            meritDict = {
                'researchprofile': self,
                'datasource': datasource,
                'organizationId': m.organizationId,
                'organizationUnitsCommaSeparated': m.organizationUnitsCommaSeparated,
                'meritName': m.meritName,
                'meritType': m.meritType,
                'externalOrganizationName': m.externalOrganizationName,
                'eventName': m.eventName,
                'eventNumber': m.eventNumber,
                'journalName': m.journalName,
                'countryCode': m.countryCode,
                'cityName': m.cityName,
                'placeName': m.placeName,
                'startYear': m.startYear,
                'startMonth': m.startMonth,
                'startDay': m.startDay,
                'endYear': m.endYear,
                'endMonth': m.endMonth,
                'endDay': m.endDay,
                'role': m.role,
                'url': m.url,
            }

            try:
                merit = Merit(**meritDict)
                merit.save()
            except Exception as e:
                print("Exception in add_aalto_data() merits")
                print(e)
                pass

        # Other Project
        for p in aalto_person.other_projects.all():
            otherprojectDict = {
                'researchprofile': self,
                'datasource': datasource,
                'organizationId': p.organizationId,
                'organizationUnitsCommaSeparated': p.organizationUnitsCommaSeparated,
                'projectName': p.projectName,
                'projectShortName': p.projectShortName,
                'projectAbbreviation': p.projectAbbreviation,
                'projectType': p.projectType,
                'startYear': p.startYear,
                'startMonth': p.startMonth,
                'startDay': p.startDay,
                'endYear': p.endYear,
                'endMonth': p.endMonth,
                'endDay': p.endDay,
                'role': p.role,
            }

            try:
                otherProject = OtherProject(**otherprojectDict)
                otherProject.save()
            except Exception as e:
                print("Exception in add_aalto_data() projects")
                print(e)
                pass

        # Research material
        for r in aalto_person.research_materials.all():
            researchmaterialDict = {
                'researchprofile': self,
                'datasource': datasource,
                'organizationId': r.organizationId,
                'name': r.name,
                'description': r.description,
                'coverageYearStart': r.coverageYearStart,
                'coverageYearEnd': r.coverageYearEnd,
                'publicationYear': r.publicationYear,
                'publisherName': r.publisherName,
                'doi': r.doi,
                'linksCommaSeparated': r.linksCommaSeparated,
                'orgUnitsCommaSeparated': r.orgUnitsCommaSeparated,
                'rolesCommaSeparated': r.rolesCommaSeparated
            }

            try:
                researchmaterial = ResearchMaterial(**researchmaterialDict)
                researchmaterial.save()
            except Exception as e:
                print("Exception in add_aalto_data() researchmaterials")
                print(e)
                pass
        self.save()

    def delete_aalto_data(self):
        self.is_aalto = False
        datasource = Datasource.objects.get(name="ORG1")
        self.last_names.filter(datasource=datasource).delete()
        self.first_names.filter(datasource=datasource).delete()
        self.other_names.filter(datasource=datasource).delete()
        self.links.filter(datasource=datasource).delete()
        self.emails.filter(datasource=datasource).delete()
        self.phones.filter(datasource=datasource).delete()
        self.biographies.filter(datasource=datasource).delete()
        self.employment.filter(datasource=datasource).delete()
        self.education.filter(datasource=datasource).delete()
        self.keywords.filter(datasource=datasource).delete()
        self.merits.filter(datasource=datasource).delete()
        self.other_projects.filter(datasource=datasource).delete()
        self.research_materials.filter(datasource=datasource).delete()
        self.save()

    def auto_include_in_profile(self):
        handleLastNames = True
        handleFirstNames = True
        handleOtherNames = True
        handleLinks = True
        handleEmails = True
        handlePhones = True
        handleBiographies = True
        handleEmployment = True
        handleEducation = True
        handleKeywords = True
        handleMerits = True
        handleOtherProjects = True
        handleResearchMaterials = True

        datasources = []
        for item in self.user.orcid_permission.get_priority_list():
            ds = Datasource.objects.get(name=item["name"])
            datasources.append(ds)

        for ds in datasources:
            # last names
            if handleLastNames and self.last_names.filter(datasource=ds).count() > 0:
                self.last_names.filter(datasource=ds).update(includeInProfile=True)
                handleLastNames = False

            # first names
            if handleFirstNames and self.first_names.filter(datasource=ds).count() > 0:
                self.first_names.filter(datasource=ds).update(includeInProfile=True)
                handleFirstNames = False

            # other names
            if handleOtherNames and self.other_names.filter(datasource=ds).count() > 0:
                self.other_names.filter(datasource=ds).update(includeInProfile=True)
                handleOtherNames = False

            # links
            if handleLinks and self.links.filter(datasource=ds).count() > 0:
                self.links.filter(datasource=ds).update(includeInProfile=True)
                handleLinks = False

            # emails
            if handleEmails and self.emails.filter(datasource=ds).count() > 0:
                self.emails.filter(datasource=ds).update(includeInProfile=True)
                handleEmails = False

            # phones
            if handlePhones and self.phones.filter(datasource=ds).count() > 0:
                self.phones.filter(datasource=ds).update(includeInProfile=True)
                handlePhones = False

            # biographies
            if handleBiographies and self.biographies.filter(datasource=ds).count() > 0:
                self.biographies.filter(datasource=ds).update(includeInProfile=True)
                handleBiographies = False

            # employment
            if handleEmployment and self.employment.filter(datasource=ds).count() > 0:
                self.employment.filter(datasource=ds).update(includeInProfile=True)
                handleEmployment = False

            # education
            if handleEducation and self.education.filter(datasource=ds).count() > 0:
                self.education.filter(datasource=ds).update(includeInProfile=True)
                handleEducation = False

            # keywords
            if handleKeywords and self.keywords.filter(datasource=ds).count() > 0:
                self.keywords.filter(datasource=ds).update(includeInProfile=True)
                handleKeywords = False

            # merits
            if handleMerits and self.merits.filter(datasource=ds).count() > 0:
                self.merits.filter(datasource=ds).update(includeInProfile=True)
                handleMerits = False

            # other projects
            if handleOtherProjects and self.other_projects.filter(datasource=ds).count() > 0:
                self.other_projects.filter(datasource=ds).update(includeInProfile=True)
                handleOtherProjects = False

            # research materials
            if handleResearchMaterials and self.research_materials.filter(datasource=ds).count() > 0:
                self.research_materials.filter(datasource=ds).update(includeInProfile=True)
                handleResearchMaterials = False

        self.save()


    def update_sources(self):
        self.delete_orcid_data()
        self.delete_org1_data()
        self.delete_org2_data()

        if self.user.orcid_permission.read_all_orcid:
            self.get_orcid_data()
        if self.user.orcid_permission.read_all_org1:
            self.add_org1_data()
        if self.user.orcid_permission.read_all_org2:
            self.add_org2_data()

        self.auto_include_in_profile()

def create_researchprofile(sender, instance, created, **kwargs):
    if created:
        ResearchProfile.objects.create(user=instance)

post_save.connect(create_researchprofile, sender=User)

class TrustedParty(models.Model):
    name = models.CharField(max_length=255)

    def __str__(self):
       return self.name

class PortalPermission(models.Model):
    user = models.ForeignKey(User, on_delete=models.CASCADE, related_name='portal_permission')
    trusted_party = models.ForeignKey(TrustedParty, on_delete=models.CASCADE, related_name='portal_permission')
    show_person_info = models.BooleanField(default=True, verbose_name='Yhteystiedot')
    show_research_description = models.BooleanField(default=True, verbose_name='Tutkimustoiminnan kuvaus')
    show_organization = models.BooleanField(default=True, verbose_name='Affiliaatio')
    show_education = models.BooleanField(default=True, verbose_name='Koulutus')
    show_works = models.BooleanField(default=True, verbose_name='Julkaisut')
    show_research_resources = models.BooleanField(default=True, verbose_name='Tutkimusaineistot')
    show_fundings = models.BooleanField(default=True, verbose_name='Hankkeet')
    show_infrastuctures = models.BooleanField(default=True, verbose_name='Tutkimusinfrastruktuurit')
    show_other_activities = models.BooleanField(default=True, verbose_name='Muut tutkimusaktiviteetit')

    class Meta:
        unique_together = ['user', 'trusted_party']

    def __str__(self):
       return self.user.username + ' - ' + self.trusted_party.name

def create_portal_permission(sender, instance, created, **kwargs):
    if created:
        for tp in TrustedParty.objects.all():
            PortalPermission.objects.create(user=instance, trusted_party=tp)

post_save.connect(create_portal_permission, sender=User)

class Position(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='%(class)s')
    datasource = models.ForeignKey(Datasource, on_delete=models.CASCADE, null=True)
    roleTitle = models.CharField(max_length=512, blank=True, null=True)
    organizationName = models.CharField(max_length=512, blank=True, null=True)
    departmentName = models.CharField(max_length=512, blank=True, null=True)
    startYear = models.PositiveSmallIntegerField(null=True)
    startMonth = models.PositiveSmallIntegerField(null=True)
    startDay = models.PositiveSmallIntegerField(null=True)
    endYear = models.PositiveSmallIntegerField(null=True)
    endMonth = models.PositiveSmallIntegerField(null=True)
    endDay = models.PositiveSmallIntegerField(null=True)
    includeInProfile = models.BooleanField(default=False)

    class Meta:
        abstract = True
        ordering = ['-startYear', '-startMonth', '-startDay']

    def get_start_date_string(self):
        startDateString = ''
        if self.startYear is not None:
            startDateString += str(self.startYear)
            
            if self.startMonth is not None:
                startDateString = str(self.startMonth) + '.' + startDateString

                if self.startDay is not None:
                    startDateString = str(self.startDay) + '.' + startDateString
        return startDateString

    def get_end_date_string(self):
        endDateString = ''
        if self.endYear is not None:
            endDateString += str(self.endYear)
            
            if self.endMonth is not None:
                endDateString = str(self.endMonth) + '.' + endDateString

                if self.endDay is not None:
                    endDateString = str(self.endDay) + '.' + endDateString
        return endDateString

class Education(Position):
    pass

class Employment(Position):
    pass

class InvitedPosition(Position):
    pass

class Funding(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='fundings')
    datasource = models.ForeignKey(Datasource, on_delete=models.CASCADE, null=True)

class Membership(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='memberships')
    datasource = models.ForeignKey(Datasource, on_delete=models.CASCADE, null=True)

class PeerReview(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='peer_reviews')
    datasource = models.ForeignKey(Datasource, on_delete=models.CASCADE, null=True)
    reviewerRole = models.CharField(max_length=512, blank=True, null=True)
    reviewUrl = models.CharField(max_length=512, blank=True, null=True)
    reviewType = models.CharField(max_length=512, blank=True, null=True)
    completionDate = models.DateField(auto_now=False, auto_now_add=False, blank=True, null=True)

    class Meta:
        ordering = ['-completionDate']

class Publication(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='publications')
    datasources = models.ManyToManyField(Datasource)
    name = models.CharField(max_length=512, blank=True)
    publicationYear = models.PositiveSmallIntegerField(null=True)
    doi = models.CharField(max_length=512, blank=True, null=True)
    includeInProfile = models.BooleanField(default=False)

    class Meta:
        ordering = ['-publicationYear']
        unique_together = [['doi', 'name']]

class Qualifications(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='qualifications')
    datasource = models.ForeignKey(Datasource, on_delete=models.CASCADE, null=True)

class ResearchResouce(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='research_resources')
    datasource = models.ForeignKey(Datasource, on_delete=models.CASCADE, null=True)

class Service(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='services')
    datasource = models.ForeignKey(Datasource, on_delete=models.CASCADE, null=True)

class PersonLastName(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='last_names')
    datasource = models.ForeignKey(Datasource, on_delete=models.CASCADE, null=True)
    includeInProfile = models.BooleanField(default=False)
    value = models.CharField(max_length=256)

    def toggleInclude(self):
        self.includeInProfile = not self.includeInProfile
        self.save()
        return self.includeInProfile

class PersonFirstName(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='first_names')
    datasource = models.ForeignKey(Datasource, on_delete=models.CASCADE, null=True)
    includeInProfile = models.BooleanField(default=False)
    value = models.CharField(max_length=256)

    def toggleInclude(self):
        self.includeInProfile = not self.includeInProfile
        self.save()
        return self.includeInProfile

class PersonOtherName(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='other_names')
    datasource = models.ForeignKey(Datasource, on_delete=models.CASCADE, null=True)
    includeInProfile = models.BooleanField(default=False)
    value = models.CharField(max_length=256)

    def toggleInclude(self):
        self.includeInProfile = not self.includeInProfile
        self.save()
        return self.includeInProfile

class PersonLink(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='links')
    datasource = models.ForeignKey(Datasource, on_delete=models.CASCADE, null=True)
    includeInProfile = models.BooleanField(default=False)
    url = models.CharField(max_length=2048, null=False)
    name = models.CharField(max_length=512, null=False)

    def toggleInclude(self):
        self.includeInProfile = not self.includeInProfile
        self.save()
        return self.includeInProfile

class PersonEmail(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='emails')
    datasource = models.ForeignKey(Datasource, on_delete=models.CASCADE, null=True)
    includeInProfile = models.BooleanField(default=False)
    value = models.CharField(max_length=512)

    def toggleInclude(self):
        self.includeInProfile = not self.includeInProfile
        self.save()
        return self.includeInProfile

class PersonPhone(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='phones')
    datasource = models.ForeignKey(Datasource, on_delete=models.CASCADE, null=True)
    includeInProfile = models.BooleanField(default=False)
    value = models.CharField(max_length=128)

    def toggleInclude(self):
        self.includeInProfile = not self.includeInProfile
        self.save()
        return self.includeInProfile

class PersonBiography(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='biographies')
    datasource = models.ForeignKey(Datasource, on_delete=models.CASCADE, null=True)
    includeInProfile = models.BooleanField(default=False)
    value = models.TextField(null=True, blank=True)

class PersonKeyword(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='keywords')
    datasource = models.ForeignKey(Datasource, on_delete=models.CASCADE, null=True)
    includeInProfile = models.BooleanField(default=False)
    value = models.CharField(max_length=128)

class ResearchMaterial(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='research_materials')
    datasource = models.ForeignKey(Datasource, on_delete=models.CASCADE, null=True)
    organizationId = models.PositiveSmallIntegerField(null=True)
    name = models.CharField(max_length=512, blank=True)
    description = models.TextField(blank=True)
    coverageYearStart = models.PositiveSmallIntegerField(null=True) 
    coverageYearEnd = models.PositiveSmallIntegerField(null=True)
    publicationYear = models.PositiveSmallIntegerField(null=True)
    publisherName = models.CharField(max_length=512, blank=True)
    doi = models.CharField(max_length=512, blank=True, null=True)
    linksCommaSeparated = models.CharField(max_length=2000, blank=True)
    orgUnitsCommaSeparated = models.CharField(max_length=1000, blank=True)
    rolesCommaSeparated = models.CharField(max_length=512, blank=True)
    includeInProfile = models.BooleanField(default=False)

class Merit(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='merits')
    datasource = models.ForeignKey(Datasource, on_delete=models.CASCADE, null=True)
    organizationId = models.PositiveSmallIntegerField(null=True)
    organizationUnitsCommaSeparated = models.CharField(max_length=512, blank=True, null=True)
    meritName = models.CharField(max_length=512, blank=True, null=True)
    meritType = models.CharField(max_length=512, blank=True, null=True)
    externalOrganizationName = models.CharField(max_length=512, blank=True, null=True)
    eventName = models.CharField(max_length=512, blank=True, null=True)
    eventNumber = models.PositiveSmallIntegerField(null=True)
    journalName = models.CharField(max_length=512, blank=True, null=True)
    countryCode = models.PositiveSmallIntegerField(null=True)
    cityName = models.CharField(max_length=256, blank=True, null=True)
    placeName = models.CharField(max_length=256, blank=True, null=True)
    startYear = models.PositiveSmallIntegerField(null=True)
    startMonth = models.PositiveSmallIntegerField(null=True)
    startDay = models.PositiveSmallIntegerField(null=True)
    endYear = models.PositiveSmallIntegerField(null=True)
    endMonth = models.PositiveSmallIntegerField(null=True)
    endDay = models.PositiveSmallIntegerField(null=True)
    role = models.CharField(max_length=512, blank=True, null=True)
    url = models.CharField(max_length=512, blank=True, null=True)
    includeInProfile = models.BooleanField(default=False)

    def getDateString(self):
        startStr = None
        endStr = None
        dateStr = None

        if self.startYear is not None:
            startStr = str(self.startYear)
            if self.startMonth is not None:
                startStr = str(self.startMonth) + '.' + startStr
                if self.startDay is not None:
                    startStr = str(self.startDay) + '.' + startStr

        if self.endYear is not None:
            endStr = str(self.endYear)
            if self.endMonth is not None:
                endStr = str(self.endMonth) + '.' + endStr
                if self.endDay is not None:
                    endStr = str(self.endDay) + '.' + endStr

        if startStr is not None and endStr is not None and startStr == endStr:
            dateStr = startStr
        elif startStr is not None and endStr is not None:
            dateStr = startStr + ' - ' + endStr
        elif startStr is not None and endStr is None:
            dateStr = startStr
        elif startStr is None and endStr is not None:
            dateStr = endStr

        return dateStr

class OtherProject(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='other_projects')
    datasource = models.ForeignKey(Datasource, on_delete=models.CASCADE, null=True)
    organizationId = models.PositiveSmallIntegerField(null=True)
    organizationUnitsCommaSeparated = models.CharField(max_length=512, blank=True, null=True)
    projectName = models.CharField(max_length=512, blank=True, null=True)
    projectShortName = models.CharField(max_length=512, blank=True, null=True)
    projectAbbreviation = models.CharField(max_length=512, blank=True, null=True)
    projectType = models.CharField(max_length=512, blank=True, null=True)
    startYear = models.PositiveSmallIntegerField(null=True)
    startMonth = models.PositiveSmallIntegerField(null=True)
    startDay = models.PositiveSmallIntegerField(null=True)
    endYear = models.PositiveSmallIntegerField(null=True)
    endMonth = models.PositiveSmallIntegerField(null=True)
    endDay = models.PositiveSmallIntegerField(null=True)
    role = models.CharField(max_length=512, blank=True, null=True)
    includeInProfile = models.BooleanField(default=False)

    def getDateString(self):
        startStr = None
        endStr = None
        dateStr = None

        if self.startYear is not None:
            startStr = str(self.startYear)
            if self.startMonth is not None:
                startStr = str(self.startMonth) + '.' + startStr
                if self.startDay is not None:
                    startStr = str(self.startDay) + '.' + startStr

        if self.endYear is not None:
            endStr = str(self.endYear)
            if self.endMonth is not None:
                endStr = str(self.endMonth) + '.' + endStr
                if self.endDay is not None:
                    endStr = str(self.endDay) + '.' + endStr

        if startStr is not None and endStr is not None and startStr == endStr:
            dateStr = startStr
        elif startStr is not None and endStr is not None:
            dateStr = startStr + ' - ' + endStr
        elif startStr is not None and endStr is None:
            dateStr = startStr
        elif startStr is None and endStr is not None:
            dateStr = endStr

        return dateStr