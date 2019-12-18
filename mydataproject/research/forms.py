from django.forms.models import modelformset_factory
from .models import PortalPermission

PortalPermissionFormSet = modelformset_factory(PortalPermission, exclude=('user', 'trusted_party'), extra=0)