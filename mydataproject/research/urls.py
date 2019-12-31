from django.urls import path

from . import views

urlpatterns = [
    path('logout', views.logout_view, name='logout_view'),
    path('delete', views.delete_profile, name='delete_profile'),
    path('settings', views.settings, name='settings'),
    path('', views.index, name='index'),
]