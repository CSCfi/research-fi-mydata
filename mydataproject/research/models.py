from django.db import models
from django.db.models.signals import post_save
from django.contrib.auth.models import User
from django.forms import ModelForm
import requests, json

class Origin(models.Model):
    name = models.CharField(max_length=256)

class ResearchProfile(models.Model):
    user = models.OneToOneField(User, on_delete=models.CASCADE, related_name='researchprofile')
    active = models.BooleanField(default=False)
    activity_distinctions = models.TextField(null=True, blank=True)
    activity_fundings = models.TextField(null=True, blank=True)
    activity_invited_positions = models.TextField(null=True, blank=True)
    activity_memberships = models.TextField(null=True, blank=True)
    activity_peer_reviews = models.TextField(null=True, blank=True)
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

    def orcid_record_json_to_model(self, orcid_record):
        if orcid_record["activities-summary"]:
            # Distinctions
            if self.user.orcid_permission.get_activities_distinctions and orcid_record["activities-summary"]["distinctions"]:
                self.activity_distinctions = json.dumps(orcid_record["activities-summary"]["distinctions"]["affiliation-group"])
            # Educations
            if self.user.orcid_permission.get_activities_educations and len(orcid_record["activities-summary"]["educations"]["affiliation-group"]) > 0:
                # Create education objects
                for affiliationGroup in orcid_record["activities-summary"]["educations"]["affiliation-group"]:
                    for summary in affiliationGroup['summaries']:
                        if summary['education-summary']:
                            # Role title
                            try:
                                if summary['education-summary']['role-title']:
                                    roleTitle = summary['education-summary']['role-title']
                                else:
                                    roleTitle = ''
                            except:
                                print("Cannot get education-summary.role-title")
                                roleTitle = ''
                                pass

                            # Organization name
                            try:
                                if summary['education-summary']['organization']['name']:
                                    organizationName = summary['education-summary']['organization']['name']
                                else:
                                    organizationName = ''
                            except:
                                print("Cannot get education-summary.organization.name")
                                organizationName = ''
                                pass

                            # End year
                            try:
                                endYear = summary['education-summary']['end-date']['year']['value']
                            except:
                                print("Cannot get education-summary.end-date.year.value")
                                endYear = None
                                pass

                            Education.objects.create(
                                researchprofile = self,
                                roleTitle = roleTitle,
                                organizationName = organizationName,
                                endYear = endYear
                            )
            # Employments
            if self.user.orcid_permission.get_activities_employments and len(orcid_record["activities-summary"]["employments"]["affiliation-group"]) > 0:
                # Create employment objects
                for affiliationGroup in orcid_record["activities-summary"]["employments"]["affiliation-group"]:
                    for summary in affiliationGroup['summaries']:
                        if summary['employment-summary']:
                            # Role title
                            try:
                                if summary['employment-summary']['role-title']:
                                    roleTitle = summary['employment-summary']['role-title']
                                else:
                                    roleTitle = ''
                            except:
                                print("Cannot get employment-summary.role-title")
                                roleTitle = ''
                                pass

                            # Organization name
                            try:
                                if summary['employment-summary']['organization']['name']:
                                    organizationName = summary['employment-summary']['organization']['name']
                                else:
                                    organizationName = ''
                            except:
                                print("Cannot get employment-summary.organization.name")
                                organizationName = ''
                                pass

                            # Department name
                            try:
                                if summary['employment-summary']['department-name']:
                                    departmentName = summary['employment-summary']['department-name']
                                else:
                                    departmentName = ''
                            except:
                                print("Cannot get employment-summary.department-name")
                                departmentName = ''
                                pass

                            # Start year
                            try:
                                startYear = summary['employment-summary']['start-date']['year']['value']
                            except:
                                print("Cannot get employment-summary.start-date.year.value")
                                startYear = None
                                pass

                            # End year
                            try:
                                endYear = summary['employment-summary']['end-date']['year']['value']
                            except:
                                print("Cannot get employment-summary.end-date.year.value")
                                endYear = None
                                pass
 
                            Employment.objects.create(
                                researchprofile = self,
                                roleTitle = roleTitle,
                                organizationName = organizationName,
                                departmentName = departmentName,
                                startYear = startYear,
                                endYear = endYear
                            )
            # Fundings
            if self.user.orcid_permission.get_activities_fundings and orcid_record["activities-summary"]["fundings"]:
                self.activity_fundings = json.dumps(orcid_record["activities-summary"]["fundings"]["group"])
            # Invited positions
            if self.user.orcid_permission.get_activities_invited_positions and orcid_record["activities-summary"]["invited-positions"]:
                self.activity_invited_positions = json.dumps(orcid_record["activities-summary"]["invited-positions"]["affiliation-group"])
            # Peer reviews
            if self.user.orcid_permission.get_activities_peer_reviews and orcid_record["activities-summary"]["peer-reviews"]:
                self.activity_peer_reviews = json.dumps(orcid_record["activities-summary"]["peer-reviews"]["group"])
            # Qualifications
            if self.user.orcid_permission.get_activities_works and orcid_record["activities-summary"]["qualifications"]:
                self.activity_qualifications = json.dumps(orcid_record["activities-summary"]["qualifications"]["affiliation-group"])
            # Research resources
            if self.user.orcid_permission.get_activities_works and orcid_record["activities-summary"]["research-resources"]:
                self.activity_research_resources = json.dumps(orcid_record["activities-summary"]["research-resources"]["group"])
            # Services
            if self.user.orcid_permission.get_activities_works and orcid_record["activities-summary"]["services"]:
                self.activity_services = json.dumps(orcid_record["activities-summary"]["services"]["affiliation-group"])
            # Works
            if self.user.orcid_permission.get_activities_works and len(orcid_record["activities-summary"]["works"]["group"]) > 0:
                # Create publication objects
                origin_orcid = Origin.objects.get(name="ORCID")
                for obj in orcid_record["activities-summary"]["works"]["group"]:
                    Publication.objects.create(
                        researchprofile = self,
                        name = obj["work-summary"][0]["title"]["title"]["value"],
                        publicationYear = obj["work-summary"][0]["publication-date"]["year"]["value"],
                        origin = origin_orcid,
                        includeInProfile = False
                    )

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

    def get_orcid_data(self):
        social = self.user.social_auth.get(provider='orcid')
        token = social.extra_data['access_token']
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
                print("ORCID record:")
                print(json_data)
                self.orcid_record_json_to_model(json_data)
            except:
                pass

        return True

    def get_virta_publications(self):
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
                print("VIRTA json:")
                print(virta_json_data)
                self.virta_publications = json.dumps(virta_json_data)
                self.save()

                # Create publication objects
                origin_ttv = Origin.objects.get(name="TTV")
                for obj in virta_json_data:
                    Publication.objects.create(
                        researchprofile = self,
                        name = obj["julkaisunNimi"],
                        publicationYear = obj["julkaisuVuosi"],
                        origin = origin_ttv,
                        includeInProfile = False
                    )
            except:
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

class Publication(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='publications')
    name = models.CharField(max_length=512, blank=True)
    publicationYear = models.PositiveSmallIntegerField(null=True)
    origin = models.ForeignKey(Origin, on_delete=models.CASCADE, null=True)
    includeInProfile = models.BooleanField(default=False)

    class Meta:
        ordering = ['-publicationYear']

class Education(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='educations')
    roleTitle = models.CharField(max_length=512, blank=True)
    organizationName = models.CharField(max_length=512, blank=True)
    endYear = models.PositiveSmallIntegerField(null=True)

    class Meta:
        ordering = ['-endYear']

class Employment(models.Model):
    researchprofile = models.ForeignKey(ResearchProfile, on_delete=models.CASCADE, related_name='employments')
    roleTitle = models.CharField(max_length=512, blank=True)
    organizationName = models.CharField(max_length=512, blank=True)
    departmentName = models.CharField(max_length=512, blank=True)
    startYear = models.PositiveSmallIntegerField(null=True)
    endYear = models.PositiveSmallIntegerField(null=True)

    class Meta:
        ordering = ['-endYear']