from django.db import models

# Create your models here.
class Person(models.Model):
    orcid = models.CharField(max_length=19, null=False)
    first_name = models.CharField(max_length=64, null=True)
    last_name = models.CharField(max_length=256, null=True)
    email = models.CharField(max_length=256, null=True)
    biography = models.TextField(null=True)

    class Meta:
        indexes = [
            models.Index(fields=['orcid'])
        ]

class Link(models.Model):
    person = models.ForeignKey(Person, on_delete=models.CASCADE, related_name='links')
    url = models.CharField(max_length=2048, null=False)
    name = models.CharField(max_length=256, null=False)

class Keyword(models.Model):
    person = models.ForeignKey(Person, on_delete=models.CASCADE, related_name='keywords')
    value = models.CharField(max_length=128, null=False)

class Affiliation(models.Model):
    person = models.ForeignKey(Person, on_delete=models.CASCADE, related_name='affiliations')
    title = models.CharField(max_length=512, null=False)
    department_name = models.CharField(max_length=1024, null=False)

class Education(models.Model):
    person = models.ForeignKey(Person, on_delete=models.CASCADE, related_name='educations')
    title = models.CharField(max_length=512, null=False)
    organization_name = models.CharField(max_length=1024, null=False)

class Publication(models.Model):
    person = models.ForeignKey(Person, on_delete=models.CASCADE, related_name='publications')
    name = models.CharField(max_length=512, blank=True)
    publicationYear = models.PositiveSmallIntegerField(null=True)
    doi = models.CharField(max_length=512, blank=True, null=True)

class ResearchMaterial(models.Model):
    person = models.ForeignKey(Person, on_delete=models.CASCADE, related_name='research_materials')
    organizationId = models.PositiveSmallIntegerField(null=True)
    name = models.CharField(max_length=512, blank=True)
    description = models.TextField(blank=True)
    coverageYearStart = models.PositiveSmallIntegerField(null=True) 
    coverageYearEnd = models.PositiveSmallIntegerField(null=True)
    publicationYear = models.PositiveSmallIntegerField(null=True)
    publisherName = models.CharField(max_length=512, blank=True, null=True)
    doi = models.CharField(max_length=512, blank=True, null=True)
    linksCommaSeparated = models.CharField(max_length=2000, blank=True, null=True)
    orgUnitsCommaSeparated = models.CharField(max_length=1000, blank=True, null=True)
    rolesCommaSeparated = models.CharField(max_length=512, blank=True, null=True)

class Merit(models.Model):
    person = models.ForeignKey(Person, on_delete=models.CASCADE, related_name='merits')
    organizationId = models.PositiveSmallIntegerField(null=True)
    organizationUnitsCommaSeparated = models.CharField(max_length=512, blank=True, null=True)
    meritName = models.CharField(max_length=512, blank=True, null=True)
    meritType = models.CharField(max_length=512, blank=True, null=True)
    externalOrganizationName = models.CharField(max_length=512, blank=True, null=True)
    eventName = models.CharField(max_length=512, blank=True, null=True)
    eventNumber = models.PositiveSmallIntegerField(null=True)
    journalName = models.CharField(max_length=512, blank=True, null=True)
    countryCode = models.PositiveSmallIntegerField(null=True)
    cityName = models.CharField(max_length=256, blank=True, null=True)
    placeName = models.CharField(max_length=256, blank=True, null=True)
    startYear = models.PositiveSmallIntegerField(null=True)
    startMonth = models.PositiveSmallIntegerField(null=True)
    startDay = models.PositiveSmallIntegerField(null=True)
    endYear = models.PositiveSmallIntegerField(null=True)
    endMonth = models.PositiveSmallIntegerField(null=True)
    endDay = models.PositiveSmallIntegerField(null=True)
    role = models.CharField(max_length=512, blank=True, null=True)
    url = models.CharField(max_length=512, blank=True, null=True)

class Project(models.Model):
    person = models.ForeignKey(Person, on_delete=models.CASCADE, related_name='projects')
    organizationId = models.PositiveSmallIntegerField(null=True)
    organizationUnitsCommaSeparated = models.CharField(max_length=512, blank=True, null=True)
    projectName = models.CharField(max_length=512, blank=True, null=True)
    projectShortName = models.CharField(max_length=512, blank=True, null=True)
    projectAbbreviation = models.CharField(max_length=512, blank=True, null=True)
    projectType = models.CharField(max_length=512, blank=True, null=True)
    startYear = models.PositiveSmallIntegerField(null=True)
    startMonth = models.PositiveSmallIntegerField(null=True)
    startDay = models.PositiveSmallIntegerField(null=True)
    endYear = models.PositiveSmallIntegerField(null=True)
    endMonth = models.PositiveSmallIntegerField(null=True)
    endDay = models.PositiveSmallIntegerField(null=True)
    role = models.CharField(max_length=512, blank=True, null=True)