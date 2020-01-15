from django.shortcuts import render, redirect
from django.contrib.auth import logout
from django.contrib.auth.decorators import login_required
from django.forms import modelformset_factory
from django.http import JsonResponse
from django.db.models import Value
from django.db.models.functions import Coalesce
from orciddata.models import Permission, PermissionForm
from research.models import PortalPermission, Datasource, Publication, PersonLastName
import json
from .forms import PortalPermissionFormSet
from django.conf import settings
from django.core import serializers

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

                    # Get data
                    request.user.researchprofile.get_all_data()

                    # Add dummy home organization data
                    request.user.researchprofile.add_dummy_home_organization_data()

                    return redirect('index')
                else:
                    # Orcid permission form is not valid
                    return render(request, 'create_profile.html', context)
        elif request.user.researchprofile.active:
            datasource_ttv = Datasource.objects.get(name="TTV")
            datasource_orcid = Datasource.objects.get(name="ORCID")
            datasource_manual = Datasource.objects.get(name="MANUAL")

            context["included_class"] = "btn-included"
            context["excluded_class"] = "btn-excluded"

            context["TTV_API_HOST"] = settings.TTV_API_HOST
            context["datasource_ttv"] = datasource_ttv
            context["datasource_orcid"] = datasource_orcid
            context["datasource_manual"] = datasource_manual
            context["orcid_id"] = request.user.researchprofile.get_visible_orcid_id()
            context["orcid_first_names"] = request.user.researchprofile.first_names.filter(datasource=datasource_orcid)
            context["orcid_last_names"] = request.user.researchprofile.last_names.filter(datasource=datasource_orcid)
            context["orcid_other_names"] = request.user.researchprofile.other_names.filter(datasource=datasource_orcid)
            context["orcid_links"] = request.user.researchprofile.links.filter(datasource=datasource_orcid)
            context["orcid_emails"] = request.user.researchprofile.emails.filter(datasource=datasource_orcid)
            context["orcid_phones"] = request.user.researchprofile.phones.filter(datasource=datasource_orcid)
            context["orcid_biography"] = request.user.researchprofile.biographies.filter(datasource=datasource_orcid).first()
            context["orcid_keywords"] = request.user.researchprofile.keywords.filter(datasource=datasource_orcid)
            context["homeorg_first_names"] = request.user.researchprofile.first_names.filter(datasource=datasource_manual)
            context["homeorg_last_names"] = request.user.researchprofile.last_names.filter(datasource=datasource_manual)
            context["homeorg_other_names"] = request.user.researchprofile.other_names.filter(datasource=datasource_manual)
            context["homeorg_links"] = request.user.researchprofile.links.filter(datasource=datasource_manual)
            context["homeorg_emails"] = request.user.researchprofile.emails.filter(datasource=datasource_manual)
            context["homeorg_phones"] = request.user.researchprofile.phones.filter(datasource=datasource_manual)
            context["employments"] = request.user.researchprofile.employment.all().annotate(start_year_null=Coalesce('startYear', Value(-1))).order_by('-start_year_null')
            context["educations"] = request.user.researchprofile.education.all().annotate(start_year_null=Coalesce('startYear', Value(-1))).order_by('-start_year_null')
            context["peer_reviews"] = request.user.researchprofile.peer_reviews.all()
            context["invited_positions"] = request.user.researchprofile.invitedposition.all()
    return render(request, 'index_template.html', context)

def logout_view(request):
    logout(request)
    return redirect('index')

@login_required
def delete_profile(request):
    if request.user and request.user.is_authenticated:
        request.user.delete()
    return redirect('index')

@login_required
def profile_preview(request):
    context = {}
    return render(request, 'preview.html', context)

@login_required
def profile_settings(request):
    if request.user.is_authenticated and request.user.researchprofile.active:
        context = {}

        context["test_orcid_id"] = request.user.researchprofile.test_orcid_id

        if request.method == 'POST':
            if request.POST['settings_form_type'] == 'orcid_permission':
                # Handle ORCID permission form
                old_permission = Permission.objects.get(user=request.user)
                permission_form = PermissionForm(request.POST, instance=old_permission)
                
                if permission_form.is_valid():
                    # Orcid permission form is valid
                    permission_form.save()

                    # Update Orcid data
                    request.user.researchprofile.delete_orcid_data()
                    request.user.researchprofile.get_orcid_data()

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

# List user's publications
@login_required
def publication_list(request):
    publications = Publication.objects.filter(researchprofile = request.user.researchprofile).order_by('-publicationYear', 'name')

    data = serializers.serialize("json", publications)
    #queryset = Publication.objects.filter(researchprofile = request.user.researchprofile).annotate(year_null=Coalesce('publicationYear', Value(-1))).order_by('-year_null', 'name').values()
    return JsonResponse({"publications": json.loads(data) })

# Add publication into user's researchprofile
@login_required
def publication_add(request):
    newPublicationDict = json.loads(request.POST.get('publication'))
    datasource_manual = Datasource.objects.get(name="MANUAL")
    doi = newPublicationDict.get("doi", None)

    if doi is not None and len(doi) > 10:
        try:
            print(doi)
            oldPublication = Publication.objects.get(researchprofile=request.user.researchprofile, doi=doi)
            oldPublication.datasources.add(datasource_manual)
            oldPublication.save()
            return JsonResponse({})
        except Publication.DoesNotExist:
            pass
    newPublication = Publication.objects.create(
        researchprofile = request.user.researchprofile,
        name = newPublicationDict.get("publicationName", None),
        publicationYear = newPublicationDict.get("publicationYear", 0),
        doi = doi,
        includeInProfile = True
    )
    newPublication.datasources.add(datasource_manual)
    newPublication.save()
    return JsonResponse({})

# Delete manually added publication from user's researchprofile
@login_required
def publication_delete(request):
    datasource_manual = Datasource.objects.get(name="MANUAL")
    publicationId = request.POST.get('publicationId')

    publication = Publication.objects.get(id=publicationId, researchprofile=request.user.researchprofile)
    
    # Remove data source "manual" from publication
    publication.datasources.remove(datasource_manual)

    # Check if there are other datasources left for the same publication.
    if publication.datasources.count() > 0:
        # There are other datasources, update publication.
        publication.save()
    else:
        # There are no other datasources, delete publication
        publication.delete()

    return JsonResponse({})

# Include or exclude a single publication from user's researchprofile
@login_required
def publication_include(request):
    publicationId = request.POST.get('publicationId')
    include_javascript_value = request.POST.get('include')
    include = True if include_javascript_value == "true" else False
    publication = Publication.objects.get(id=publicationId, researchprofile=request.user.researchprofile)
    publication.includeInProfile = include
    publication.save()
    return JsonResponse({})

# Include or exclude all publications from user's researchprofile
@login_required
def publication_include_all(request):
    include_javascript_value = request.POST.get('include')
    include = True if include_javascript_value == "true" else False
    publications = Publication.objects.filter(researchprofile=request.user.researchprofile)
    publications.update(includeInProfile = include)
    return JsonResponse({})

@login_required
def test_orcid_id(request):
    request.user.researchprofile.test_orcid_id = request.POST.get('test_orcid_id')
    request.user.researchprofile.save()

    # Delete old data
    request.user.researchprofile.delete_all_data()
    request.user.researchprofile.include_orcid_id_in_profile = True

    # Get new data
    request.user.researchprofile.get_all_data()

    request.user.researchprofile.save()
    return redirect('index')

@login_required
def toggle_contact_info(request):
    response = {}
    objectType = request.POST.get('objectType', None)
    objectId = int(request.POST.get('objectId', '0'))

    if objectType is not None and objectId > 0:
        # Last name
        if objectType == 'lastName':
            obj = request.user.researchprofile.last_names.get(id=objectId)
            response = { 'included': obj.toggleInclude() }

        # First name
        elif objectType == 'firstName':
            obj = request.user.researchprofile.first_names.get(id=objectId)
            response = { 'included': obj.toggleInclude() }

        # Other name
        elif objectType == 'otherName':
            obj = request.user.researchprofile.other_names.get(id=objectId)
            response = { 'included': obj.toggleInclude() }
    
        # ORCID ID
        elif objectType == 'orcidId':
            request.user.researchprofile.include_orcid_id_in_profile = not request.user.researchprofile.include_orcid_id_in_profile
            request.user.researchprofile.save()
            response = { 'included': request.user.researchprofile.include_orcid_id_in_profile }

        # Link
        elif objectType == 'link':
            obj = request.user.researchprofile.links.get(id=objectId)
            response = { 'included': obj.toggleInclude() }

        # Email
        elif objectType == 'email':
            obj = request.user.researchprofile.emails.get(id=objectId)
            response = { 'included': obj.toggleInclude() }

        # Phone
        elif objectType == 'phone':
            obj = request.user.researchprofile.phones.get(id=objectId)
            response = { 'included': obj.toggleInclude() }

    return JsonResponse(response)

@login_required
def toggle_contact_info_all(request):
    response = {}
    datasource_type = request.POST.get('datasourceType', None)
    toggleJavascriptValue = request.POST.get('toggle', None)

    # Convert Javascript boolean to Python boolean
    if toggleJavascriptValue == 'true':
        toggle = True
    else:
        toggle = False

    if datasource_type == 'orcid':
        datasource = Datasource.objects.get(name="ORCID")
        request.user.researchprofile.include_orcid_id_in_profile = toggle
    elif datasource_type == 'manual':
        datasource = Datasource.objects.get(name="MANUAL")
    else:
        return JsonResponse(response)
        
    request.user.researchprofile.last_names.filter(datasource=datasource).update(includeInProfile=toggle)
    request.user.researchprofile.first_names.filter(datasource=datasource).update(includeInProfile=toggle)
    request.user.researchprofile.other_names.filter(datasource=datasource).update(includeInProfile=toggle)
    request.user.researchprofile.links.filter(datasource=datasource).update(includeInProfile=toggle)
    request.user.researchprofile.emails.filter(datasource=datasource).update(includeInProfile=toggle)
    request.user.researchprofile.phones.filter(datasource=datasource).update(includeInProfile=toggle)
    request.user.researchprofile.save()
    return JsonResponse(response)