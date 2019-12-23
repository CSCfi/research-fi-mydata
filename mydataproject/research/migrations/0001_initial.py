# Generated by Django 3.0.1 on 2019-12-23 11:03

from django.conf import settings
from django.db import migrations, models
import django.db.models.deletion


class Migration(migrations.Migration):

    initial = True

    dependencies = [
        migrations.swappable_dependency(settings.AUTH_USER_MODEL),
    ]

    operations = [
        migrations.CreateModel(
            name='Datasource',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('name', models.CharField(max_length=256)),
            ],
        ),
        migrations.CreateModel(
            name='ResearchProfile',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('active', models.BooleanField(default=False)),
                ('activity_distinctions', models.TextField(blank=True, null=True)),
                ('activity_fundings', models.TextField(blank=True, null=True)),
                ('activity_memberships', models.TextField(blank=True, null=True)),
                ('activity_qualifications', models.TextField(blank=True, null=True)),
                ('activity_research_resources', models.TextField(blank=True, null=True)),
                ('activity_services', models.TextField(blank=True, null=True)),
                ('person_addresses', models.TextField(blank=True, null=True)),
                ('person_biography', models.TextField(blank=True, null=True)),
                ('person_emails', models.TextField(blank=True, null=True)),
                ('person_external_identifiers', models.TextField(blank=True, null=True)),
                ('person_keywords', models.TextField(blank=True, null=True)),
                ('person_other_names', models.TextField(blank=True, null=True)),
                ('person_researcher_urls', models.TextField(blank=True, null=True)),
                ('virta_publications', models.TextField(blank=True, default='[]', null=True)),
                ('user', models.OneToOneField(on_delete=django.db.models.deletion.CASCADE, related_name='researchprofile', to=settings.AUTH_USER_MODEL)),
            ],
        ),
        migrations.CreateModel(
            name='TrustedParty',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('name', models.CharField(max_length=255)),
            ],
        ),
        migrations.CreateModel(
            name='Service',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='services', to='research.ResearchProfile')),
            ],
        ),
        migrations.CreateModel(
            name='ResearchResouce',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='research_resources', to='research.ResearchProfile')),
            ],
        ),
        migrations.CreateModel(
            name='Qualifications',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='qualifications', to='research.ResearchProfile')),
            ],
        ),
        migrations.CreateModel(
            name='Publication',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('name', models.CharField(blank=True, max_length=512)),
                ('publicationYear', models.PositiveSmallIntegerField(null=True)),
                ('includeInProfile', models.BooleanField(default=False)),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='publications', to='research.ResearchProfile')),
            ],
            options={
                'ordering': ['-publicationYear'],
            },
        ),
        migrations.CreateModel(
            name='PeerReview',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('reviewerRole', models.CharField(blank=True, max_length=512, null=True)),
                ('reviewUrl', models.CharField(blank=True, max_length=512, null=True)),
                ('reviewType', models.CharField(blank=True, max_length=512, null=True)),
                ('completionDate', models.DateField(blank=True, null=True)),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='peer_reviews', to='research.ResearchProfile')),
            ],
            options={
                'ordering': ['-completionDate'],
            },
        ),
        migrations.CreateModel(
            name='Membership',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='memberships', to='research.ResearchProfile')),
            ],
        ),
        migrations.CreateModel(
            name='InvitedPosition',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('roleTitle', models.CharField(blank=True, max_length=512)),
                ('organizationName', models.CharField(blank=True, max_length=512)),
                ('departmentName', models.CharField(blank=True, max_length=512)),
                ('startDate', models.DateField(blank=True, null=True)),
                ('endDate', models.DateField(blank=True, null=True)),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='invitedposition', to='research.ResearchProfile')),
            ],
            options={
                'ordering': ['-endDate'],
                'abstract': False,
            },
        ),
        migrations.CreateModel(
            name='Funding',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='fundings', to='research.ResearchProfile')),
            ],
        ),
        migrations.CreateModel(
            name='Employment',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('roleTitle', models.CharField(blank=True, max_length=512)),
                ('organizationName', models.CharField(blank=True, max_length=512)),
                ('departmentName', models.CharField(blank=True, max_length=512)),
                ('startDate', models.DateField(blank=True, null=True)),
                ('endDate', models.DateField(blank=True, null=True)),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='employment', to='research.ResearchProfile')),
            ],
            options={
                'ordering': ['-endDate'],
                'abstract': False,
            },
        ),
        migrations.CreateModel(
            name='Education',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('roleTitle', models.CharField(blank=True, max_length=512)),
                ('organizationName', models.CharField(blank=True, max_length=512)),
                ('departmentName', models.CharField(blank=True, max_length=512)),
                ('startDate', models.DateField(blank=True, null=True)),
                ('endDate', models.DateField(blank=True, null=True)),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='education', to='research.ResearchProfile')),
            ],
            options={
                'ordering': ['-endDate'],
                'abstract': False,
            },
        ),
        migrations.CreateModel(
            name='PortalPermission',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('show_person_info', models.BooleanField(default=True, verbose_name='Nimi ja tunnisteet')),
                ('show_research_description', models.BooleanField(default=True, verbose_name='Tutkimustoiminnan kuvaus')),
                ('show_organization', models.BooleanField(default=True, verbose_name='Organisaatio')),
                ('show_education', models.BooleanField(default=True, verbose_name='Tutkinnot')),
                ('show_works', models.BooleanField(default=True, verbose_name='Julkaisut')),
                ('show_research_resources', models.BooleanField(default=True, verbose_name='Tutkimusaineistot')),
                ('show_fundings', models.BooleanField(default=True, verbose_name='Hankkeet')),
                ('show_infrastuctures', models.BooleanField(default=True, verbose_name='Tutkimusinfrastruktuurit')),
                ('show_other_activities', models.BooleanField(default=True, verbose_name='Muut tutkimusaktiviteetit')),
                ('trusted_party', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='portal_permission', to='research.TrustedParty')),
                ('user', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='portal_permission', to=settings.AUTH_USER_MODEL)),
            ],
            options={
                'unique_together': {('user', 'trusted_party')},
            },
        ),
    ]
