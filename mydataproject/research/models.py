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

class ResearchProfile(models.Model):
    user = models.OneToOneField(User, on_delete=models.CASCADE, related_name='researchprofile')
    active = models.BooleanField(default=False)
    test_orcid_id = models.CharField(max_length=20, blank=True)
    include_orcid_id_in_profile = models.BooleanField(default=False)
    homeorg_datasource = models.ForeignKey(Datasource, null=True, on_delete=models.SET_NULL, related_name='researchprofile')

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
        self.delete_dummy_home_organization_data()
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

    def add_home_organization_data(self):
        orcid_id = self.get_orcid_id()
        datasource_manual = Datasource.objects.get(name="MANUAL")
        datasource_aalto = Datasource.objects.get(name="AALTO")

        print("------")
        print("Search aalto data for ORCID " + orcid_id)
        try:
            aalto_person = Person.objects.get(orcid=orcid_id)
        except Person.DoesNotExist:
            aalto_person = None

        if aalto_person is not None:
            print("AALTO home org")
            self.homeorg_datasource = datasource_aalto
            self.save()
            self.add_aalto_data(aalto_person)
        else:
            print("MANUAL home org")
            self.homeorg_datasource = datasource_manual
            self.save()
            self.add_dummy_home_organization_data()

    def get_all_data(self):
        self.get_orcid_data()
        self.get_virta_publications()
        self.add_home_organization_data()

    def add_dummy_home_organization_data(self):
        datasource_manual = Datasource.objects.get(name="MANUAL")

        # Dummy last name
        lastName = PersonLastName.objects.create(
            researchprofile = self,
            datasource = datasource_manual,
            includeInProfile = False,
            value = 'Virtanen'
        )
        self.last_names.add(lastName)

        # Dummy first name
        firstName = PersonFirstName.objects.create(
            researchprofile = self,
            datasource = datasource_manual,
            includeInProfile = False,
            value = 'A'
        )
        self.first_names.add(firstName)

        # Dummy other name
        otherName = PersonOtherName.objects.create(
            researchprofile = self,
            datasource = datasource_manual,
            includeInProfile = False,
            value = 'A.Virtanen<br>John Smith'
        )
        self.other_names.add(otherName)

        # Links
        PersonLink.objects.create(researchprofile=self, datasource=datasource_manual, includeInProfile=False, url='https://www.google.com', name='Google')
        PersonLink.objects.create(researchprofile=self, datasource=datasource_manual, includeInProfile=False, url='https://www.facebook.com', name='Facebook')
        PersonLink.objects.create(researchprofile=self, datasource=datasource_manual, includeInProfile=False, url='https://www.linkedin.com', name='LinkedIn')

        # Email
        email = PersonEmail.objects.create(
            researchprofile = self,
            datasource = datasource_manual,
            includeInProfile = False,
            value = 'abcd@example.comm'
        )
        self.emails.add(email)

        # Phone
        dummyPhoneList = [
            '+358 50 111 111 111',
            '01-54325432',
        ]
        phoneObj = PersonPhone.objects.create(
            researchprofile = self,
            datasource = datasource_manual,
            includeInProfile = False,
            value = '+358 50 111 111 111<br>05-54325432'
        )
        self.phones.add(phoneObj)

        
    
    def delete_dummy_home_organization_data(self):
        datasource_manual = Datasource.objects.get(name="MANUAL")
        self.last_names.filter(datasource=datasource_manual).delete()
        self.first_names.filter(datasource=datasource_manual).delete()
        self.other_names.filter(datasource=datasource_manual).delete()
        self.links.filter(datasource=datasource_manual).delete()
        self.emails.filter(datasource=datasource_manual).delete()
        self.phones.filter(datasource=datasource_manual).delete()

    def add_aalto_data(self, aalto_person):
        datasource_aalto = Datasource.objects.get(name="AALTO")

        # Last name
        lastName = PersonLastName.objects.create(
            researchprofile = self,
            datasource = datasource_aalto,
            includeInProfile = False,
            value = aalto_person.last_name
        )
        self.last_names.add(lastName)

        # First name
        firstName = PersonFirstName.objects.create(
            researchprofile = self,
            datasource = datasource_aalto,
            includeInProfile = False,
            value = aalto_person.first_name
        )
        self.first_names.add(firstName)

        # Email
        if aalto_person.email is not None:
            PersonEmail.objects.create(
                researchprofile = self,
                datasource = datasource_aalto,
                includeInProfile = False,
                value = aalto_person.email
            )

        # Links
        links = []
        for link in aalto_person.links.all():
            PersonLink.objects.create(
                researchprofile = self,
                datasource = datasource_aalto,
                includeInProfile = False,
                url = link.url,
                name = link.name
            )

        # Biography
        if aalto_person.biography is not None:
            PersonBiography.objects.create(
                researchprofile = self,
                datasource = datasource_aalto,
                includeInProfile = False,
                value = aalto_person.biography
            )

        # Keywords
        keywords = []
        for keyword in aalto_person.keywords.all():
            PersonKeyword.objects.create(
                researchprofile = self,
                datasource = datasource_aalto,
                includeInProfile = False,
                value = keyword.value
            )

        # Employment
        for affiliation in aalto_person.affiliations.all():
            employmentDict = {
                'researchprofile': self,
                'datasource': datasource_aalto,
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
                'datasource': datasource_aalto,
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

        # Project
        for p in aalto_person.projects.all():
            projectDict = {
                'researchprofile': self,
                'datasource': datasource_aalto,
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
                project = Project(**projectDict)
                project.save()
            except Exception as e:
                print("Exception in add_aalto_data() projects")
                print(e)
                pass

        # Research material
        for r in aalto_person.research_materials.all():
            researchmaterialDict = {
                'researchprofile': self,
                'datasource': datasource_aalto,
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

    def delete_aalto_data(self):
        datasource_aalto = Datasource.objects.get(name="AALTO")
        self.last_names.filter(datasource=datasource_aalto).delete()
        self.first_names.filter(datasource=datasource_aalto).delete()
        self.other_names.filter(datasource=datasource_aalto).delete()
        self.links.filter(datasource=datasource_aalto).delete()
        self.emails.filter(datasource=datasource_aalto).delete()
        self.phones.filter(datasource=datasource_aalto).delete()
        self.biographies.filter(datasource=datasource_aalto).delete()
        self.employment.filter(datasource=datasource_aalto).delete()
        self.education.filter(datasource=datasource_aalto).delete()
        self.keywords.filter(datasource=datasource_aalto).delete()
        self.merits.filter(datasource=datasource_aalto).delete()
        self.projects.filter(datasource=datasource_aalto).delete()
        self.research_materials.filter(datasource=datasource_aalto).delete()

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

class Project(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='projects')
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