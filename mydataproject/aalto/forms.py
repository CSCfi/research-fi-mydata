from django import forms

class UploadResearchersForm(forms.Form):
    xmlfile = forms.FileField()

class UploadPublicationsForm(forms.Form):
    xmlfile = forms.FileField()

class UploadResearchmaterialsForm(forms.Form):
    xmlfile = forms.FileField()