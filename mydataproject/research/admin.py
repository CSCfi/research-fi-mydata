from django.contrib import admin
from .models import TrustedParty, PortalPermission

admin.site.register(TrustedParty)
admin.site.register(PortalPermission)