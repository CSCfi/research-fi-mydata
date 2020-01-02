from django.shortcuts import render, redirect
from django.contrib.auth import logout
from django.forms import modelformset_factory
from orciddata.models import Permission, PermissionForm
from research.models import PortalPermission, Datasource, Publication
import json
from .forms import PortalPermissionFormSet
from django.conf import settings

def index(request):
    context = {}

    # Check if user is authenticated and if the user has active profile
    if request.user.is_authenticated:
        if not request.user.researchprofile.active:
            # Authenticated user does not have active profile
            if request.method == "GET":
                permission = Permission.objects.get(user=request.user)
                context['permission_form'] = PermissionForm(instance=permission, label_suffix='')
                return render(request, 'create_profile.html', context)
            elif request.method == "POST":
                old_permission = Permission.objects.get(user=request.user)
                permission_form = PermissionForm(request.POST, instance=old_permission)
                
                if permission_form.is_valid():
                    # Orcid permission form is valid
                    permission_form.save()

                    # Mark profile active
                    request.user.researchprofile.active = True
                    request.user.researchprofile.save()

                    # Get public data from ORCID
                    request.user.researchprofile.get_orcid_data()

                    # Get VIRTA publications
                    request.user.researchprofile.get_virta_publications()

                    return redirect('index')
                else:
                    # Orcid permission form is not valid
                    return render(request, 'create_profile.html', context)
        elif request.user.researchprofile.active:
            context["TTV_API_HOST"] = settings.TTV_API_HOST
            context["datasource_ttv"] = Datasource.objects.get(name="TTV")
            context["datasource_orcid"] = Datasource.objects.get(name="ORCID")
            context["employments"] = request.user.researchprofile.employment.all()
            context["educations"] = request.user.researchprofile.education.all()
            context["publications"] = request.user.researchprofile.publications.all().order_by('-publicationYear', 'name')
            #context["publications_ttv"] = request.user.researchprofile.publications.filter(datasource = datasource_ttv)
            #context["publications_orcid"] = request.user.researchprofile.publications.filter(datasource = datasource_orcid)
            context["peer_reviews"] = request.user.researchprofile.peer_reviews.all()
            context["invited_positions"] = request.user.researchprofile.invitedposition.all()

            if request.method == "POST":
                #old_portal_permission = PortalPermission.objects.get(user=request.user)
                #portal_permission_form = PortalPermissionForm(request.POST, instance=old_portal_permission)

                portal_permission_formset = PortalPermissionFormSet(request.POST, queryset=PortalPermission.objects.filter(user=request.user))

                if portal_permission_formset.is_valid():
                    # Portal permission form is valid
                    portal_permission_formset.save()
                    return redirect('index')
                else:
                    # Portal permission form is not valid
                    print(portal_permission_formset.errors)
                    context['portal_permission_formset'] = portal_permission_formset
            else:
                context['portal_permission_formset'] = PortalPermissionFormSet(queryset=PortalPermission.objects.filter(user=request.user))

                #portal_permission = PortalPermission.objects.get(user=request.user)
                #context['portal_permission_form'] = PortalPermissionForm(instance=portal_permission, label_suffix='')

    return render(request, 'index_template.html', context)

def logout_view(request):
    logout(request)
    return redirect('index')

def delete_profile(request):
    if request.user and request.user.is_authenticated:
        request.user.delete()
    return redirect('index')

def profile_settings(request):
    if request.user.is_authenticated and request.user.researchprofile.active:
        context = {}
        if request.method == 'POST':
            if request.POST['settings_form_type'] == 'orcid_permission':
                # Handle ORCID permission form
                old_permission = Permission.objects.get(user=request.user)
                permission_form = PermissionForm(request.POST, instance=old_permission)
                
                if permission_form.is_valid():
                    # Orcid permission form is valid
                    permission_form.save()
                    return redirect('profile_settings')
            elif request.POST['settings_form_type'] == 'profile_read':
                # Handle profile read permission form
                portal_permission_formset = PortalPermissionFormSet(request.POST, queryset=PortalPermission.objects.filter(user=request.user))

                if portal_permission_formset.is_valid():
                    # Portal permission form is valid
                    portal_permission_formset.save()
                    return redirect('profile_settings')
                else:
                    # Portal permission form is not valid
                    print(portal_permission_formset.errors)
                    context['portal_permission_formset'] = portal_permission_formset

        # ORCID permissions
        if not 'permission_form' in context:
            permission = Permission.objects.get(user=request.user)
            context['permission_form'] = PermissionForm(instance=permission, label_suffix='')

        # Sharing permissions
        if not 'portal_permission_formset' in context:
            context['portal_permission_formset'] = PortalPermissionFormSet(queryset=PortalPermission.objects.filter(user=request.user))

        return render(request, 'settings.html', context)
    else:
        return redirect('index')
