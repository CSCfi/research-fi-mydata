# Generated by Django 3.0.1 on 2020-05-20 13:10

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('research', '0001_initial'),
    ]

    operations = [
        migrations.CreateModel(
            name='AreaOfInterest',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('name', models.CharField(max_length=256)),
            ],
        ),
        migrations.AddField(
            model_name='researchprofile',
            name='areas_of_interest',
            field=models.ManyToManyField(to='research.AreaOfInterest'),
        ),
    ]
