from django.urls import path

from . import views

urlpatterns = [
    path('logout', views.logout_view, name='logout_view'),
    path('delete', views.delete_profile, name='delete_profile'),
    path('preview', views.profile_preview, name='profile_preview'),
    path('settings', views.profile_settings, name='profile_settings'),
    path('publication_list', views.publication_list, name='publication_list'),
    path('publication_include', views.publication_include, name='publication_include'),
    path('publication_include_all', views.publication_include_all, name='publication_include_all'),
    path('publication_add', views.publication_add, name='publication_add'),
    path('publication_delete', views.publication_delete, name='publication_delete'),
    path('', views.index, name='index'),
]