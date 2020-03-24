
from django.shortcuts import render, redirect
from django.contrib.auth.decorators import login_required
from aalto.forms import UploadResearchersForm, UploadPublicationsForm, UploadResearchmaterialsForm, UploadMeritsForm, UploadOtherProjectsForm
from aalto.parse_xml_publications import parse_publications
from aalto.parse_xml_researchers import parse_researchers
from aalto.parse_xml_researchmaterials import parse_researchmaterials
from aalto.parse_xml_merits import parse_merits
from aalto.parse_xml_otherprojects import parse_otherprojects

@login_required
def upload_researchers(request):
    if request.method == 'POST':
        form = UploadResearchersForm(request.POST, request.FILES)
        if form.is_valid():
            parse_researchers(request.FILES['xmlfile'])
            return redirect('index')
    else:
        form = UploadResearchersForm()
    return render(request, 'upload_researchers.html', {'form': form})

@login_required
def upload_publications(request):
    if request.method == 'POST':
        form = UploadPublicationsForm(request.POST, request.FILES)
        if form.is_valid():
            parse_publications(request.FILES['xmlfile'])
            return redirect('index')
    else:
        form = UploadPublicationsForm()
    return render(request, 'upload_publications.html', {'form': form})

@login_required
def upload_researchmaterials(request):
    if request.method == 'POST':
        form = UploadResearchmaterialsForm(request.POST, request.FILES)
        if form.is_valid():
            parse_researchmaterials(request.FILES['xmlfile'])
            return redirect('index')
    else:
        form = UploadResearchmaterialsForm()
    return render(request, 'upload_researchmaterials.html', {'form': form})

@login_required
def upload_merits(request):
    if request.method == 'POST':
        form = UploadMeritsForm(request.POST, request.FILES)
        if form.is_valid():
            parse_merits(request.FILES['xmlfile'])
            return redirect('index')
    else:
        form = UploadMeritsForm()
    return render(request, 'upload_merits.html', {'form': form})

@login_required
def upload_otherprojects(request):
    if request.method == 'POST':
        form = UploadOtherProjectsForm(request.POST, request.FILES)
        if form.is_valid():
            parse_otherprojects(request.FILES['xmlfile'])
            return redirect('index')
    else:
        form = UploadOtherProjectsForm()
    return render(request, 'upload_otherprojects.html', {'form': form})