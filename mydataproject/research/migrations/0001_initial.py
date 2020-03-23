# Generated by Django 3.0.1 on 2020-03-20 10:47

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
                ('test_orcid_id', models.CharField(blank=True, max_length=20)),
                ('include_orcid_id_in_profile', models.BooleanField(default=False)),
                ('homeorg_datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.SET_NULL, related_name='researchprofile', to='research.Datasource')),
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
            name='ResearchMaterial',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('organizationId', models.PositiveSmallIntegerField(null=True)),
                ('name', models.CharField(blank=True, max_length=512)),
                ('description', models.TextField(blank=True)),
                ('coverageYearStart', models.PositiveSmallIntegerField(null=True)),
                ('coverageYearEnd', models.PositiveSmallIntegerField(null=True)),
                ('publicationYear', models.PositiveSmallIntegerField(null=True)),
                ('publisherName', models.CharField(blank=True, max_length=512)),
                ('doi', models.CharField(blank=True, max_length=512, null=True)),
                ('linksCommaSeparated', models.CharField(blank=True, max_length=2000)),
                ('orgUnitsCommaSeparated', models.CharField(blank=True, max_length=1000)),
                ('rolesCommaSeparated', models.CharField(blank=True, max_length=512)),
                ('includeInProfile', models.BooleanField(default=False)),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='research_materials', to='research.ResearchProfile')),
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
            name='PersonPhone',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('includeInProfile', models.BooleanField(default=False)),
                ('value', models.CharField(max_length=128)),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='phones', to='research.ResearchProfile')),
            ],
        ),
        migrations.CreateModel(
            name='PersonOtherName',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('includeInProfile', models.BooleanField(default=False)),
                ('value', models.CharField(max_length=256)),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='other_names', to='research.ResearchProfile')),
            ],
        ),
        migrations.CreateModel(
            name='PersonLink',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('includeInProfile', models.BooleanField(default=False)),
                ('url', models.CharField(max_length=2048)),
                ('name', models.CharField(max_length=512)),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='links', to='research.ResearchProfile')),
            ],
        ),
        migrations.CreateModel(
            name='PersonLastName',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('includeInProfile', models.BooleanField(default=False)),
                ('value', models.CharField(max_length=256)),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='last_names', to='research.ResearchProfile')),
            ],
        ),
        migrations.CreateModel(
            name='PersonKeyword',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('includeInProfile', models.BooleanField(default=False)),
                ('value', models.CharField(max_length=128)),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='keywords', to='research.ResearchProfile')),
            ],
        ),
        migrations.CreateModel(
            name='PersonFirstName',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('includeInProfile', models.BooleanField(default=False)),
                ('value', models.CharField(max_length=256)),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='first_names', to='research.ResearchProfile')),
            ],
        ),
        migrations.CreateModel(
            name='PersonEmail',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('includeInProfile', models.BooleanField(default=False)),
                ('value', models.CharField(max_length=512)),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='emails', to='research.ResearchProfile')),
            ],
        ),
        migrations.CreateModel(
            name='PersonBiography',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('includeInProfile', models.BooleanField(default=False)),
                ('value', models.TextField(blank=True, null=True)),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='biographies', to='research.ResearchProfile')),
            ],
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
            name='Merit',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('organizationId', models.PositiveSmallIntegerField(null=True)),
                ('organizationUnitsCommaSeparated', models.CharField(blank=True, max_length=512, null=True)),
                ('meritName', models.CharField(blank=True, max_length=512, null=True)),
                ('meritType', models.CharField(blank=True, max_length=512, null=True)),
                ('externalOrganizationName', models.CharField(blank=True, max_length=512, null=True)),
                ('eventName', models.CharField(blank=True, max_length=512, null=True)),
                ('eventNumber', models.PositiveSmallIntegerField(null=True)),
                ('journalName', models.CharField(blank=True, max_length=512, null=True)),
                ('countryCode', models.PositiveSmallIntegerField(null=True)),
                ('cityName', models.CharField(blank=True, max_length=256, null=True)),
                ('placeName', models.CharField(blank=True, max_length=256, null=True)),
                ('startYear', models.PositiveSmallIntegerField(null=True)),
                ('startMonth', models.PositiveSmallIntegerField(null=True)),
                ('startDay', models.PositiveSmallIntegerField(null=True)),
                ('endYear', models.PositiveSmallIntegerField(null=True)),
                ('endMonth', models.PositiveSmallIntegerField(null=True)),
                ('endDay', models.PositiveSmallIntegerField(null=True)),
                ('role', models.CharField(blank=True, max_length=512, null=True)),
                ('url', models.CharField(blank=True, max_length=512, null=True)),
                ('includeInProfile', models.BooleanField(default=False)),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='merits', to='research.ResearchProfile')),
            ],
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
                ('roleTitle', models.CharField(blank=True, max_length=512, null=True)),
                ('organizationName', models.CharField(blank=True, max_length=512, null=True)),
                ('departmentName', models.CharField(blank=True, max_length=512, null=True)),
                ('startYear', models.PositiveSmallIntegerField(null=True)),
                ('startMonth', models.PositiveSmallIntegerField(null=True)),
                ('startDay', models.PositiveSmallIntegerField(null=True)),
                ('endYear', models.PositiveSmallIntegerField(null=True)),
                ('endMonth', models.PositiveSmallIntegerField(null=True)),
                ('endDay', models.PositiveSmallIntegerField(null=True)),
                ('includeInProfile', models.BooleanField(default=False)),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='invitedposition', to='research.ResearchProfile')),
            ],
            options={
                'ordering': ['-startYear', '-startMonth', '-startDay'],
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
                ('roleTitle', models.CharField(blank=True, max_length=512, null=True)),
                ('organizationName', models.CharField(blank=True, max_length=512, null=True)),
                ('departmentName', models.CharField(blank=True, max_length=512, null=True)),
                ('startYear', models.PositiveSmallIntegerField(null=True)),
                ('startMonth', models.PositiveSmallIntegerField(null=True)),
                ('startDay', models.PositiveSmallIntegerField(null=True)),
                ('endYear', models.PositiveSmallIntegerField(null=True)),
                ('endMonth', models.PositiveSmallIntegerField(null=True)),
                ('endDay', models.PositiveSmallIntegerField(null=True)),
                ('includeInProfile', models.BooleanField(default=False)),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='employment', to='research.ResearchProfile')),
            ],
            options={
                'ordering': ['-startYear', '-startMonth', '-startDay'],
                'abstract': False,
            },
        ),
        migrations.CreateModel(
            name='Education',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('roleTitle', models.CharField(blank=True, max_length=512, null=True)),
                ('organizationName', models.CharField(blank=True, max_length=512, null=True)),
                ('departmentName', models.CharField(blank=True, max_length=512, null=True)),
                ('startYear', models.PositiveSmallIntegerField(null=True)),
                ('startMonth', models.PositiveSmallIntegerField(null=True)),
                ('startDay', models.PositiveSmallIntegerField(null=True)),
                ('endYear', models.PositiveSmallIntegerField(null=True)),
                ('endMonth', models.PositiveSmallIntegerField(null=True)),
                ('endDay', models.PositiveSmallIntegerField(null=True)),
                ('includeInProfile', models.BooleanField(default=False)),
                ('datasource', models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='education', to='research.ResearchProfile')),
            ],
            options={
                'ordering': ['-startYear', '-startMonth', '-startDay'],
                'abstract': False,
            },
        ),
        migrations.CreateModel(
            name='Publication',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('name', models.CharField(blank=True, max_length=512)),
                ('publicationYear', models.PositiveSmallIntegerField(null=True)),
                ('doi', models.CharField(blank=True, max_length=512, null=True)),
                ('includeInProfile', models.BooleanField(default=False)),
                ('datasources', models.ManyToManyField(to='research.Datasource')),
                ('researchprofile', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='publications', to='research.ResearchProfile')),
            ],
            options={
                'ordering': ['-publicationYear'],
                'unique_together': {('doi', 'name')},
            },
        ),
        migrations.CreateModel(
            name='PortalPermission',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('show_person_info', models.BooleanField(default=True, verbose_name='Yhteystiedot')),
                ('show_research_description', models.BooleanField(default=True, verbose_name='Tutkimustoiminnan kuvaus')),
                ('show_organization', models.BooleanField(default=True, verbose_name='Affiliaatio')),
                ('show_education', models.BooleanField(default=True, verbose_name='Koulutus')),
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
