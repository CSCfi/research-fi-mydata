
from django.shortcuts import render, redirect
from django.contrib.auth.decorators import login_required
from aalto.forms import UploadFileForm
from aalto.parse_xml import handle_upload

@login_required
def upload_file(request):
    if request.method == 'POST':
        form = UploadFileForm(request.POST, request.FILES)
        if form.is_valid():
            handle_upload(request.FILES['xmlfile'])
            return redirect('index')
    else:
        form = UploadFileForm()
    return render(request, 'upload.html', {'form': form})