from django.db import models
from django.db.models.signals import post_save
from django.contrib.auth.models import User
from django.forms import ModelForm
import requests, json
import datetime

# For quick testing
orcid_id_test = None

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
    person_addresses = models.TextField(null=True, blank=True)
    person_biography = models.TextField(null=True, blank=True)
    person_emails = models.TextField(null=True, blank=True)
    person_external_identifiers = models.TextField(null=True, blank=True)
    person_keywords = models.TextField(null=True, blank=True)
    person_other_names = models.TextField(null=True, blank=True)
    person_researcher_urls = models.TextField(null=True, blank=True)
    virta_publications = models.TextField(null=True, blank=True, default='[]')

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
            if self.user.orcid_permission.get_activities_fundings and orcid_record["activities-summary"]["fundings"]:
                self.activity_fundings = json.dumps(orcid_record["activities-summary"]["fundings"]["group"])
            # Invited positions
            if self.user.orcid_permission.get_activities_invited_positions and len(orcid_record["activities-summary"]["invited-positions"]["affiliation-group"]) > 0:
                for affiliationGroup in orcid_record["activities-summary"]["invited-positions"]["affiliation-group"]:
                    for summary in affiliationGroup['summaries']:
                        invitedPositionObj = self.getPositionObject(summary.get('invited-position-summary', None))
                        if invitedPositionObj is not None:
                            invitedPositionObj['datasource'] = datasource_orcid
                            try:
                                invitedPosition = InvitedPosition(**invitedPositionObj)
                                invitedPosition.save()
                            except Exception as e:
                                print("Exception in orcid_record_json_to_model() invited positions")
                                print(e)
                                pass
                
            # Peer reviews
            try:
                if self.user.orcid_permission.get_activities_peer_reviews and len(orcid_record["activities-summary"]["peer-reviews"]["group"]) > 0:
                        for group in orcid_record["activities-summary"]["peer-reviews"]["group"]:
                            for peerReviewGroup in group["peer-review-group"]:
                                for peerReviewSummary in peerReviewGroup["peer-review-summary"]:          
                                    PeerReview.objects.create(
                                        researchprofile = self,
                                        datasource = datasource_orcid,
                                        reviewerRole = peerReviewSummary.get('reviewer-role', ''),
                                        reviewUrl = peerReviewSummary.get('review-url', ''),
                                        reviewType = peerReviewSummary.get('review-type', ''),
                                        completionDate = self.getDate(peerReviewSummary['completion-date'])
                                )
            except Exception as e:
                print("Exception in orcid_record_json_to_model() peer reviews")
                print(e)
                pass

            # Qualifications
            try:
                if self.user.orcid_permission.get_activities_works and orcid_record["activities-summary"]["qualifications"]:
                    self.activity_qualifications = json.dumps(orcid_record["activities-summary"]["qualifications"]["affiliation-group"])
            except Exception as e:
                print("Exception in orcid_record_json_to_model() qualifications")
                print(e)
                pass

            # Research resources
            try:
                if self.user.orcid_permission.get_activities_works and orcid_record["activities-summary"]["research-resources"]:
                    self.activity_research_resources = json.dumps(orcid_record["activities-summary"]["research-resources"]["group"])
            except Exception as e:
                print("Exception in orcid_record_json_to_model() research resources")
                print(e)
                pass

            # Services
            try:
                if self.user.orcid_permission.get_activities_works and orcid_record["activities-summary"]["services"]:
                    self.activity_services = json.dumps(orcid_record["activities-summary"]["services"]["affiliation-group"])
            except Exception as e:
                print("Exception in orcid_record_json_to_model() services")
                print(e)
                pass

            # Works
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
                        try:
                            Publication.objects.create(
                                researchprofile = self,
                                name = obj["work-summary"][0]["title"]["title"]["value"],
                                publicationYear = obj["work-summary"][0]["publication-date"]["year"]["value"] if obj["work-summary"][0]["publication-date"] is not None else None,
                                doi = doi,
                                datasource = datasource_orcid,
                                includeInProfile = True
                            )
                        except Exception as e:
                            print("Exception in orcid_record_json_to_model() works, when creating Publication object")
                            print(e)
                            pass

            except Exception as e:
                print("Exception in orcid_record_json_to_model() works")
                print(e)
                pass

        if orcid_record["person"]:
            # Addresses
            if self.user.orcid_permission.get_person_addresses and orcid_record["person"]["addresses"]:
                self.person_addresses = json.dumps(orcid_record["person"]["addresses"]["address"])
            # Biography
            if self.user.orcid_permission.get_person_biography and orcid_record["person"]["biography"] and orcid_record["person"]["biography"]["content"]:
                self.person_biography = orcid_record["person"]["biography"]["content"]
            # Emails
            if self.user.orcid_permission.get_person_emails and len(orcid_record["person"]["emails"]["email"]) > 0:
                emaillist = []
                for obj in orcid_record["person"]["emails"]["email"]:
                    emaillist.append(obj["email"])
                self.person_emails = ', '.join(emaillist)
            # External identifiers
            if self.user.orcid_permission.get_person_external_identifiers and len(orcid_record["person"]["external-identifiers"]["external-identifier"]) > 0:
                identifierlist = []
                for obj in orcid_record["person"]["external-identifiers"]["external-identifier"]:
                    url = "<a href='" + obj["external-id-url"]["value"] + "' target='_blank'>" + obj["external-id-type"] + "</a>"
                    identifierlist.append(url)
                self.person_external_identifiers = ', '.join(identifierlist)
            # Keywords
            if self.user.orcid_permission.get_person_keywords and len(orcid_record["person"]["keywords"]["keyword"]) > 0:
                keywordlist = []
                for obj in orcid_record["person"]["keywords"]["keyword"]:
                    keywordlist.append(obj["content"])
                self.person_keywords = ', '.join(keywordlist)
            # Other names
            if self.user.orcid_permission.get_person_other_names and len(orcid_record["person"]["other-names"]["other-name"]) > 0:
                namelist = []
                for obj in orcid_record["person"]["other-names"]["other-name"]:
                    namelist.append(obj["content"])
                self.person_other_names = ', '.join(namelist)
            # Researcher URLs
            if self.user.orcid_permission.get_person_researcher_urls and len(orcid_record["person"]["researcher-urls"]["researcher-url"]) > 0:
                urllist = []
                for obj in orcid_record["person"]["researcher-urls"]["researcher-url"]:
                    url = "<a href='" + obj["url"]["value"] + "' target='_blank'>" + obj["url-name"] + "</a>"
                    urllist.append(url)
                self.person_researcher_urls = ', '.join(urllist)
        self.save()

    def delete_orcid_data(self):
        datasource_orcid = Datasource.objects.get(name="ORCID")
        Education.objects.filter(researchprofile = self).delete()
        Employment.objects.filter(researchprofile = self).delete()
        Funding.objects.filter(researchprofile = self).delete()
        InvitedPosition.objects.filter(researchprofile = self).delete()
        Membership.objects.filter(researchprofile = self).delete()
        PeerReview.objects.filter(researchprofile = self).delete()
        Publication.objects.filter(researchprofile = self, datasource=datasource_orcid).delete()
        Qualifications.objects.filter(researchprofile = self).delete()
        ResearchResouce.objects.filter(researchprofile = self).delete()
        Service.objects.filter(researchprofile = self).delete()

    def get_orcid_data(self):
        social = self.user.social_auth.get(provider='orcid')
        token = social.extra_data['access_token']

        if orcid_id_test is not None:
            orcid_id = orcid_id_test
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
        if orcid_id_test is not None:
            orcid_id = orcid_id_test
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
                        Publication.objects.create(
                            researchprofile = self,
                            name = obj.get("julkaisunNimi", None),
                            publicationYear = obj.get("julkaisuVuosi", None),
                            doi = obj.get("doi", None),
                            datasource = datasource_ttv,
                            includeInProfile = True
                        )
                    except Exception as e:
                        print("Exception when creating Publication objects from Virta json")
                        print(e)
                        pass
            except Exception as e:
                print("Exception in get_virta_publications()")
                print(e)
                pass

        return True

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
    show_person_info = models.BooleanField(default=True, verbose_name='Nimi ja tunnisteet')
    show_research_description = models.BooleanField(default=True, verbose_name='Tutkimustoiminnan kuvaus')
    show_organization = models.BooleanField(default=True, verbose_name='Organisaatio')
    show_education = models.BooleanField(default=True, verbose_name='Tutkinnot')
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
    datasource = models.ForeignKey(Datasource, on_delete=models.CASCADE, null=True)
    name = models.CharField(max_length=512, blank=True)
    publicationYear = models.PositiveSmallIntegerField(null=True)
    doi = models.CharField(max_length=512, blank=True, null=True)
    includeInProfile = models.BooleanField(default=False)

    class Meta:
        ordering = ['-publicationYear']
        unique_together = [['name', 'datasource']]

class Qualifications(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='qualifications')
    datasource = models.ForeignKey(Datasource, on_delete=models.CASCADE, null=True)

class ResearchResouce(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='research_resources')
    datasource = models.ForeignKey(Datasource, on_delete=models.CASCADE, null=True)

class Service(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='services')
    datasource = models.ForeignKey(Datasource, on_delete=models.CASCADE, null=True)