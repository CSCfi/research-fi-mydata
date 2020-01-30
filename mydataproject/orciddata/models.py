from django.db import models
from django.db.models.signals import post_save
from django.contrib.auth.models import User
from django.forms import ModelForm
from django.utils.translation import gettext_lazy as _


class Permission(models.Model):
    user = models.OneToOneField(User, on_delete=models.CASCADE, related_name='orcid_permission')
    read_all = models.BooleanField(default=False)

def create_permission(sender, instance, created, **kwargs):
    if created:
        Permission.objects.create(user=instance)

post_save.connect(create_permission, sender=User)

class PermissionForm(ModelForm):
    class Meta:
        model = Permission
        fields = [
            'read_all',
        ]
        labels = {
            'read_all': _('Tuo kaikki ORCID-tiedot'),
        }