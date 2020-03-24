from django.urls import path

from . import views

urlpatterns = [
    path('upload_researchers', views.upload_researchers, name='upload_researchers'),
    path('upload_publications', views.upload_publications, name='upload_publications'),
    path('upload_researchmaterials', views.upload_researchmaterials, name='upload_researchmaterials'),
    path('upload_merits', views.upload_merits, name='upload_merits'),
    path('upload_otherprojects', views.upload_otherprojects, name='upload_otherprojects'),
]