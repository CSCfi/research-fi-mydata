from django.urls import path

from . import views

urlpatterns = [
    path('logout', views.logout_view, name='logout_view'),
    path('delete', views.delete_profile, name='delete_profile'),
    path('settings', views.profile_settings, name='profile_settings'),
    path('publication_add', views.publication_add, name='publication_add'),
    path('', views.index, name='index'),
]