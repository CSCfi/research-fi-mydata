# Generated by Django 3.0.1 on 2020-03-20 10:21

from django.db import migrations, models
import django.db.models.deletion


class Migration(migrations.Migration):

    initial = True

    dependencies = [
    ]

    operations = [
        migrations.CreateModel(
            name='Affiliation',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('title', models.CharField(max_length=512)),
                ('department_name', models.CharField(max_length=1024)),
            ],
        ),
        migrations.CreateModel(
            name='Education',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('title', models.CharField(max_length=512)),
                ('organization_name', models.CharField(max_length=1024)),
            ],
        ),
        migrations.CreateModel(
            name='Keyword',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('value', models.CharField(max_length=128)),
            ],
        ),
        migrations.CreateModel(
            name='Link',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('url', models.CharField(max_length=2048)),
                ('name', models.CharField(max_length=256)),
            ],
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
            ],
        ),
        migrations.CreateModel(
            name='Person',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('orcid', models.CharField(max_length=19)),
                ('first_name', models.CharField(max_length=64, null=True)),
                ('last_name', models.CharField(max_length=256, null=True)),
                ('email', models.CharField(max_length=256, null=True)),
                ('biography', models.TextField(null=True)),
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
                ('publisherName', models.CharField(blank=True, max_length=512, null=True)),
                ('doi', models.CharField(blank=True, max_length=512, null=True)),
                ('linksCommaSeparated', models.CharField(blank=True, max_length=2000, null=True)),
                ('orgUnitsCommaSeparated', models.CharField(blank=True, max_length=1000, null=True)),
                ('rolesCommaSeparated', models.CharField(blank=True, max_length=512, null=True)),
                ('person', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='research_materials', to='aalto.Person')),
            ],
        ),
        migrations.CreateModel(
            name='Publication',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('name', models.CharField(blank=True, max_length=512)),
                ('publicationYear', models.PositiveSmallIntegerField(null=True)),
                ('doi', models.CharField(blank=True, max_length=512, null=True)),
                ('person', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='publications', to='aalto.Person')),
            ],
        ),
        migrations.AddIndex(
            model_name='person',
            index=models.Index(fields=['orcid'], name='aalto_perso_orcid_4cf2c6_idx'),
        ),
        migrations.AddField(
            model_name='merit',
            name='person',
            field=models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='merits', to='aalto.Person'),
        ),
        migrations.AddField(
            model_name='link',
            name='person',
            field=models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='links', to='aalto.Person'),
        ),
        migrations.AddField(
            model_name='keyword',
            name='person',
            field=models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='keywords', to='aalto.Person'),
        ),
        migrations.AddField(
            model_name='education',
            name='person',
            field=models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='educations', to='aalto.Person'),
        ),
        migrations.AddField(
            model_name='affiliation',
            name='person',
            field=models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='affiliations', to='aalto.Person'),
        ),
    ]
