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
    organization_name = models.CharField(max_length=1024, null=False)

class Education(models.Model):
    person = models.ForeignKey(Person, on_delete=models.CASCADE, related_name='educations')
    title = models.CharField(max_length=512, null=False)
    organization_name = models.CharField(max_length=1024, null=False)