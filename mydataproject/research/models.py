from django.db import models
from django.db.models.signals import post_save
from django.contrib.auth.models import User
from django.forms import ModelForm
import requests, json
import datetime

class Datasource(models.Model):
    name = models.CharField(max_length=256)

class ResearchProfile(models.Model):
    user = models.OneToOneField(User, on_delete=models.CASCADE, related_name='researchprofile')
    active = models.BooleanField(default=False)
    activity_distinctions = models.TextField(null=True, blank=True)
    activity_fundings = models.TextField(null=True, blank=True)
    activity_memberships = models.TextField(null=True, blank=True)
    activity_qualifications = models.TextField(null=True, blank=True)
    activity_research_resources = models.TextField(null=True, blank=True)
    activity_services = models.TextField(null=True, blank=True)
    virta_publications = models.TextField(null=True, blank=True, default='[]')
    test_orcid_id = models.CharField(max_length=20, blank=True)
    include_orcid_id_in_profile = models.BooleanField(default=False)

    def test_orcid_id_is_valid(self):
        return self.test_orcid_id is not None and len(self.test_orcid_id) == 19

    def get_visible_orcid_id(self):
        return self.test_orcid_id if self.test_orcid_id_is_valid() else self.user.username

    def getYearMonthDay(self, dateDict):
        string_year = None
        string_month = None
        string_day = None
        year = None
        month = None
        day = None

        if dateDict is not None:
            if dateDict.get('year') is not None:
                string_year = dateDict['year'].get('value', None)
            if dateDict.get('month') is not None:
                string_month = dateDict['month'].get('value', None)
            if dateDict.get('day') is not None:
                string_day = dateDict['day'].get('value', None)

            year = int(string_year) if string_year is not None else None
            month = int(string_month) if string_month is not None else None
            day = int(string_day) if string_day is not None else None

        return year, month, day

    def getPositionObject(self, positionDict):
        try:
            startYear, startMonth, startDay = self.getYearMonthDay(positionDict.get('start-date', None))
            endYear, endMonth, endDay = self.getYearMonthDay(positionDict.get('end-date', None))

            return {
                'researchprofile': self,
                'organizationName': positionDict['organization'].get('name', ''),
                'departmentName': positionDict.get('department-name', ''),
                'roleTitle': positionDict.get('role-title', ''),
                'startYear': startYear,
                'startMonth': startMonth,
                'startDay': startDay,
                'endYear': endYear,
                'endMonth': endMonth,
                'endDay': endDay
            }
        except Exception as e:
            print("Exception in getPositionObject()")
            print(e)
            return None

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

    def orcid_record_json_to_model(self, orcid_record):
        datasource_orcid = Datasource.objects.get(name="ORCID")

        if orcid_record["activities-summary"]:
            # Distinctions
            if self.user.orcid_permission.get_activities_distinctions and orcid_record["activities-summary"]["distinctions"]:
                self.activity_distinctions = json.dumps(orcid_record["activities-summary"]["distinctions"]["affiliation-group"])
            # Educations
            if self.user.orcid_permission.get_activities_educations and len(orcid_record["activities-summary"]["educations"]["affiliation-group"]) > 0:
                # Create education objects
                for affiliationGroup in orcid_record["activities-summary"]["educations"]["affiliation-group"]:
                    for summary in affiliationGroup['summaries']:
                        educationObj = self.getPositionObject(summary.get('education-summary', None))
                        if educationObj is not None:
                            educationObj['datasource'] = datasource_orcid
                            try:
                                education = Education(**educationObj)
                                education.save()
                            except Exception as e:
                                print("Exception in orcid_record_json_to_model() educations")
                                print(e)
                                pass

            # Employments
            if self.user.orcid_permission.get_activities_employments and len(orcid_record["activities-summary"]["employments"]["affiliation-group"]) > 0:
                # Create employment objects
                for affiliationGroup in orcid_record["activities-summary"]["employments"]["affiliation-group"]:
                    for summary in affiliationGroup['summaries']:
                        employmentObj = self.getPositionObject(summary.get('employment-summary', None))
                        if employmentObj is not None:
                            employmentObj['datasource'] = datasource_orcid
                            try:
                                employment = Employment(**employmentObj)
                                employment.save()
                            except Exception as e:
                                print("Exception in orcid_record_json_to_model() employments")
                                print(e)
                                pass
            
            # Fundings
            #if self.user.orcid_permission.get_activities_fundings and orcid_record["activities-summary"]["fundings"]:
            #    self.activity_fundings = json.dumps(orcid_record["activities-summary"]["fundings"]["group"])
            
            # Invited positions
            #if self.user.orcid_permission.get_activities_invited_positions and len(orcid_record["activities-summary"]["invited-positions"]["affiliation-group"]) > 0:
            #    for affiliationGroup in orcid_record["activities-summary"]["invited-positions"]["affiliation-group"]:
            #        for summary in affiliationGroup['summaries']:
            #            invitedPositionObj = self.getPositionObject(summary.get('invited-position-summary', None))
            #            if invitedPositionObj is not None:
            #                invitedPositionObj['datasource'] = datasource_orcid
            #                try:
            #                    invitedPosition = InvitedPosition(**invitedPositionObj)
            #                    invitedPosition.save()
            #                except Exception as e:
            #                    print("Exception in orcid_record_json_to_model() invited positions")
            #                    print(e)
            #                    pass
                
            # Peer reviews
            #try:
            #    if self.user.orcid_permission.get_activities_peer_reviews and len(orcid_record["activities-summary"]["peer-reviews"]["group"]) > 0:
            #            for group in orcid_record["activities-summary"]["peer-reviews"]["group"]:
            #                for peerReviewGroup in group["peer-review-group"]:
            #                    for peerReviewSummary in peerReviewGroup["peer-review-summary"]:          
            #                        PeerReview.objects.create(
            #                            researchprofile = self,
            #                            datasource = datasource_orcid,
            #                            reviewerRole = peerReviewSummary.get('reviewer-role', ''),
            #                            reviewUrl = peerReviewSummary.get('review-url', ''),
            #                            reviewType = peerReviewSummary.get('review-type', ''),
            #                            completionDate = self.getDate(peerReviewSummary['completion-date'])
            #                    )
            #except Exception as e:
            #    print("Exception in orcid_record_json_to_model() peer reviews")
            #    print(e)
            #    pass

            # Qualifications
            #try:
            #    if self.user.orcid_permission.get_activities_works and orcid_record["activities-summary"]["qualifications"]:
            #        self.activity_qualifications = json.dumps(orcid_record["activities-summary"]["qualifications"]["affiliation-group"])
            #except Exception as e:
            #    print("Exception in orcid_record_json_to_model() qualifications")
            #    print(e)
            #    pass

            # Research resources
            #try:
            #    if self.user.orcid_permission.get_activities_works and orcid_record["activities-summary"]["research-resources"]:
            #        self.activity_research_resources = json.dumps(orcid_record["activities-summary"]["research-resources"]["group"])
            #except Exception as e:
            #    print("Exception in orcid_record_json_to_model() research resources")
            #    print(e)
            #    pass

            # Services
            #try:
            #    if self.user.orcid_permission.get_activities_works and orcid_record["activities-summary"]["services"]:
            #        self.activity_services = json.dumps(orcid_record["activities-summary"]["services"]["affiliation-group"])
            #except Exception as e:
            #    print("Exception in orcid_record_json_to_model() services")
            #    print(e)
            #    pass

            # Works (=Publications)
            try:
                if self.user.orcid_permission.get_activities_works and len(orcid_record["activities-summary"]["works"]["group"]) > 0:
                    # Create publication objects
                    for obj in orcid_record["activities-summary"]["works"]["group"]:
                        # Parse DOI
                        try:
                            doi = None
                            if "external-id" in obj["external-ids"]:
                                for external_id in obj["external-ids"]["external-id"]:
                                    if external_id["external-id-type"] == "doi":
                                        doi = external_id.get("external-id-value", None)
                        except Exception as e:
                            print("Exception in orcid_record_json_to_model() works, when parsing DOI")
                            print(e)
                            pass

                        # Create object
                        name = obj["work-summary"][0]["title"]["title"]["value"]
                        publicationYear = obj["work-summary"][0]["publication-date"]["year"]["value"] if obj["work-summary"][0]["publication-date"] is not None else 0
                        self.update_or_create_publication(
                            doi,
                            datasource_orcid,
                            name,
                            publicationYear,
                            True
                        )
            except Exception as e:
                print("Exception in orcid_record_json_to_model() works")
                print(e)
                pass

        if orcid_record["person"]:
            # Name
            if self.user.orcid_permission.get_person_name:
                # Fist name
                try:
                    PersonFirstName.objects.create(
                        researchprofile = self,
                        datasource = datasource_orcid,
                        includeInProfile = False,
                        value = orcid_record["person"]["name"]["given-names"]["value"]
                    )
                except Exception as e:
                    print("Exception in orcid_record_json_to_model() first name")
                    print(e)
                    pass
                
                # Last name
                try:
                    PersonLastName.objects.create(
                        researchprofile = self,
                        datasource = datasource_orcid,
                        includeInProfile = False,
                        value = orcid_record["person"]["name"]["family-name"]["value"]
                    )
                except Exception as e:
                    print("Exception in orcid_record_json_to_model() last name")
                    print(e)
                    pass

            # Other names
            if self.user.orcid_permission.get_person_other_names and len(orcid_record["person"]["other-names"]["other-name"]) > 0:
                other_names = []
                for obj in orcid_record["person"]["other-names"]["other-name"]:
                    other_names.append(obj["content"])
                if len(other_names) > 0:
                    try:
                        PersonOtherName.objects.create(
                            researchprofile = self,
                            datasource = datasource_orcid,
                            includeInProfile = False,
                            value = "<br>".join(other_names)
                        )
                    except Exception as e:
                        print("Exception in orcid_record_json_to_model() other names")
                        print(e)
                        pass

            # Biography
            if self.user.orcid_permission.get_person_biography and orcid_record["person"]["biography"] and orcid_record["person"]["biography"]["content"]:
                try:
                    PersonBiography.objects.create(
                        researchprofile = self,
                        datasource = datasource_orcid,
                        includeInProfile = False,
                        value = orcid_record["person"]["biography"]["content"]
                    )
                except Exception as e:
                    print("Exception in orcid_record_json_to_model() biography")
                    print(e)
                    pass
            # Emails
            if self.user.orcid_permission.get_person_emails and len(orcid_record["person"]["emails"]["email"]) > 0:
                email_list = []
                for obj in orcid_record["person"]["emails"]["email"]:
                    email_list.append(obj['email'])
                if len(email_list) > 0:
                    try:
                        PersonEmail.objects.create(
                            researchprofile = self,
                            datasource = datasource_orcid,
                            includeInProfile = False,
                            value = "<br>".join(email_list)
                        )
                    except Exception as e:
                        print("Exception in orcid_record_json_to_model() email")
                        print(e)
                        pass
            # Keywords
            if self.user.orcid_permission.get_person_keywords and len(orcid_record["person"]["keywords"]["keyword"]) > 0:
                for obj in orcid_record["person"]["keywords"]["keyword"]:
                    try:
                        PersonKeyword.objects.create(
                            researchprofile = self,
                            datasource = datasource_orcid,
                            includeInProfile = False,
                            value = obj["content"]
                        )
                    except Exception as e:
                        print("Exception in orcid_record_json_to_model() keywords")
                        print(e)
                        pass
            # External identifiers and Researcher URLs to links
            links = []
            if self.user.orcid_permission.get_person_researcher_urls and len(orcid_record["person"]["researcher-urls"]["researcher-url"]) > 0:
                for obj in orcid_record["person"]["researcher-urls"]["researcher-url"]:
                    url = obj["url"]["value"]
                    name = obj["url-name"]
                    linkHtml = self.getLinkHtml(url, name)
                    links.append(linkHtml)
            if self.user.orcid_permission.get_person_external_identifiers and len(orcid_record["person"]["external-identifiers"]["external-identifier"]) > 0:
                for obj in orcid_record["person"]["external-identifiers"]["external-identifier"]:
                    url = obj["external-id-url"]["value"]
                    name = obj["external-id-type"]
                    linkHtml = self.getLinkHtml(url, name)
                    links.append(linkHtml)
            if len(links) > 0:
                try:
                    PersonLink.objects.create(
                        researchprofile = self,
                        datasource = datasource_orcid,
                        includeInProfile = False,
                        value = "<br>".join(links)
                    )
                except Exception as e:
                    print("Exception in orcid_record_json_to_model() external identifiers")
                    print(e)
                    pass
        self.save()

    def delete_orcid_data(self):
        datasource_orcid = Datasource.objects.get(name="ORCID")

        self.include_orcid_id_in_profile = False

        self.biographies.filter(datasource=datasource_orcid).delete()
        self.emails.filter(datasource=datasource_orcid).delete()
        self.links.filter(datasource=datasource_orcid).delete()
        self.phones.filter(datasource=datasource_orcid).delete()
        self.keywords.filter(datasource=datasource_orcid).delete()
        self.first_names.filter(datasource=datasource_orcid).delete()
        self.last_names.filter(datasource=datasource_orcid).delete()
        self.other_names.filter(datasource=datasource_orcid).delete()

        Education.objects.filter(researchprofile = self).delete()
        Employment.objects.filter(researchprofile = self).delete()
        Funding.objects.filter(researchprofile = self).delete()
        InvitedPosition.objects.filter(researchprofile = self).delete()
        Membership.objects.filter(researchprofile = self).delete()
        PeerReview.objects.filter(researchprofile = self).delete()
        Qualifications.objects.filter(researchprofile = self).delete()
        ResearchResouce.objects.filter(researchprofile = self).delete()
        Service.objects.filter(researchprofile = self).delete()

        # Delete publications whose only data source is Orcid.
        # If there are other data sources, keep the publication but remove the Orcid datasource.
        publications = Publication.objects.filter(researchprofile = self, datasources=datasource_orcid)
        for p in publications:
            if p.datasources.count() == 1:
                p.delete()
            else:
                p.datasources.remove(datasource_orcid)

        self.save()

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

    def get_orcid_data(self):
        social = self.user.social_auth.get(provider='orcid')
        token = social.extra_data['access_token']

        if self.test_orcid_id_is_valid():
            orcid_id = self.test_orcid_id 
        else:
            orcid_id = self.user.username

        # Get public data
        headers = {
            'Accept': 'application/json',
            'Authorization type': 'Bearer',
            'Access token': token
        }

        # ORCID API URL
        url = 'https://pub.orcid.org/v3.0/' + orcid_id + '/record'
        print("ORCID RECORD URL = " + url)

        response = requests.get(url, headers=headers)
        print("ORCID http response code " + str(response.status_code))
        if response.status_code == 200:
            try:
                json_data = response.json()
                self.orcid_record_json_to_model(json_data)
            except Exception as e:
                print("Exception in get_orcid_data()")
                print(e)
                pass

        return True

    def get_virta_publications(self):
        if self.test_orcid_id_is_valid():
            orcid_id = self.test_orcid_id 
        else:
            orcid_id = self.user.username

        headers = {
            'Accept': 'application/json',
        }

        # VIRTA API (json)
        url = 'https://virta-jtp.csc.fi/api/julkaisut/haku?orcid=' + orcid_id
        print("VIRTA URL = " + url)
        response = requests.get(url, headers=headers)
        print("VIRTA http response code " + str(response.status_code))
        if response.status_code == 200:
            try:
                virta_json_data = response.json()
                self.virta_publications = json.dumps(virta_json_data)
                self.save()

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

    def get_all_data(self):
        self.get_orcid_data()
        self.get_virta_publications()
        self.add_dummy_home_organization_data()

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

        # Link
        dummyLinkList = [
            self.getLinkHtml('https://www.google.fi', 'Google'),
            self.getLinkHtml('https://www.facebook.com', 'Facebook'),
            self.getLinkHtml('https://www.linkedin.com', 'LinkedIn'),
        ]
        linkObj = PersonLink.objects.create(
            researchprofile = self,
            datasource = datasource_manual,
            includeInProfile = False,
            value = "<br>".join(dummyLinkList)
        )
        self.links.add(linkObj)

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
    value = models.CharField(max_length=2048)

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