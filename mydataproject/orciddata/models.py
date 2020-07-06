from django.db import models
from django.db.models.signals import post_save
from django.contrib.auth.models import User
from django.forms import ModelForm
from django.utils.translation import gettext_lazy as _


class Permission(models.Model):
    user = models.OneToOneField(User, on_delete=models.CASCADE, related_name='orcid_permission')
    read_all_orcid = models.BooleanField(default=False)
    read_all_org1 = models.BooleanField(default=False)
    read_all_org2 = models.BooleanField(default=False)
    priority_orcid = models.PositiveSmallIntegerField(null=True)
    priority_org1 = models.PositiveSmallIntegerField(null=True)
    priority_org2 = models.PositiveSmallIntegerField(null=True)

def create_permission(sender, instance, created, **kwargs):
    if created:
        Permission.objects.create(user=instance)

post_save.connect(create_permission, sender=User)

class PermissionForm(ModelForm):
    class Meta:
        model = Permission
        fields = [
            'read_all_orcid',
            'read_all_org1',
            'read_all_org2',
            'priority_orcid',
            'priority_org1',
            'priority_org2',
        ]
        labels = {
            'read_all_orcid': _('Tuo kaikki ORCID-tiedot'),
            'read_all_org1': _('Tuo kaikki tiedot organisaatiosta 1'),
            'read_all_org2': _('Tuo kaikki tiedot organisaatiosta 2'),
            'priority_orcid': _('Prioriteetti ORCID'),
            'priority_org1': _('Prioriteetti organisaatio 1'),
            'priority_org2': _('Prioriteetti organisaatio 2'),
        }