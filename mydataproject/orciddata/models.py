from django.db import models
from django.db.models.signals import post_save
from django.contrib.auth.models import User
from django.forms import ModelForm
from django.utils.translation import gettext_lazy as _
from research.models import ResearchProfile


class Permission(models.Model):
    user = models.OneToOneField(User, on_delete=models.CASCADE, related_name='orcid_permission')
    get_activities_distinctions = models.BooleanField(default=False)
    get_activities_educations = models.BooleanField(default=False)
    get_activities_employments = models.BooleanField(default=False)
    get_activities_fundings = models.BooleanField(default=False)
    get_activities_invited_positions = models.BooleanField(default=False)
    get_activities_memberships = models.BooleanField(default=False)
    get_activities_peer_reviews = models.BooleanField(default=False)
    get_activities_qualifications = models.BooleanField(default=False)
    get_activities_research_resources = models.BooleanField(default=False)
    get_activities_services = models.BooleanField(default=False)
    get_activities_works = models.BooleanField(default=False)
    get_person_addresses = models.BooleanField(default=False)
    get_person_biography = models.BooleanField(default=False)
    get_person_emails = models.BooleanField(default=False)
    get_person_external_identifiers = models.BooleanField(default=False)
    get_person_keywords = models.BooleanField(default=False)
    get_person_other_names = models.BooleanField(default=False)
    get_person_researcher_urls = models.BooleanField(default=False)

def create_permission(sender, instance, created, **kwargs):
    if created:
        Permission.objects.create(user=instance)

post_save.connect(create_permission, sender=User)

class PermissionForm(ModelForm):
    class Meta:
        model = Permission
        fields = [
            'get_person_addresses',
            'get_person_biography',
            'get_person_emails',
            'get_person_external_identifiers',
            'get_person_keywords',
            'get_person_other_names',
            'get_person_researcher_urls',
            'get_activities_distinctions',
            'get_activities_educations',
            'get_activities_employments',
            'get_activities_fundings',
            'get_activities_invited_positions',
            'get_activities_memberships',
            'get_activities_peer_reviews',
            'get_activities_qualifications',
            'get_activities_research_resources',
            'get_activities_services',
            'get_activities_works',
        ]
        labels = {
            'get_person_addresses': _('Osoitteet'),
            'get_person_biography': _('Biografia'),
            'get_person_emails': _('Sähköpostiosoitteet'),
            'get_person_external_identifiers': _('Muut tunnisteet'),
            'get_person_keywords': _('Avainsanat'),
            'get_person_other_names': _('Muut nimet'),
            'get_person_researcher_urls': _('Verkko-osoitteet'),
            'get_activities_distinctions': _('Meriitit'),
            'get_activities_educations': _('Tutkinnot'),
            'get_activities_employments': _('Organisaatiot'),
            'get_activities_fundings': _('Rahoitukset'),
            'get_activities_invited_positions': _('Kutsutut tehtävät'),
            'get_activities_memberships': _('Jäsenyydet'),
            'get_activities_peer_reviews': _('Vertaisarviot'),
            'get_activities_qualifications': _('Pätevyydet'),
            'get_activities_research_resources': _('Tutkimusresurssit'),
            'get_activities_services': _('Palvelut'),
            'get_activities_works': _('Julkaisut'),
        }