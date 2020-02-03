# python ../manage.py shell < parse_xml.py

import xml.etree.ElementTree as ET
from aalto.models import *

def handle_upload(f):
    count_new = 0
    count_update = 0

    for line in f:
        orcid = None
        first_name = None
        last_name = None
        email = None
        biography = None
        links = []
        keywords = []
        affiliations = []
        educations = []

        if line.startswith(b'<Tutkija>'):

            try:
                root = ET.fromstring(line)
            except Exception as e:
                print("Exception when parsing file " + filename)
                print(e)
                continue

            for child in root:
                if child.tag == 'ORCID':
                    orcid = child.text
                elif child.tag == 'Etunimet':
                    first_name = child.text
                elif child.tag == 'Sukunimi':
                    last_name = child.text
                elif child.tag == 'SahkopostiosoiteTeksti':
                    email = child.text
                elif child.tag == 'Kuvaus':
                    biography = child.text
                elif child.tag == 'Avainsanat':
                    for av in child.findall('AvainsanaTeksti'):
                        keywords.append(av.text)
                elif child.tag == 'VerkkoOsoitteet':
                    for ve in child.findall('VerkkoOsoite'):
                        link = {}
                        for ve_child in ve:
                            if ve_child.tag == 'VerkkoOsoiteTeksti':
                                link['url'] = ve_child.text
                            elif ve_child.tag == 'VerkkoOsoiteTyyppi':
                                link['name'] = ve_child.text
                        links.append(link)

                elif child.tag == 'Affiliaatiot':
                    for af in child.findall('Affiliaatio'):
                        affiliation = {}
                        for af_child in af:
                            if af_child.tag == 'TehtavanimikeNimi':
                                affiliation['title'] = af_child.text
                            elif af_child.tag == 'YksikkoKoodi':
                                affiliation['department_name'] = af_child.text
                        affiliations.append(affiliation)
                elif child.tag == 'Koulutukset':
                    for ko in child.findall('Koulutus'):
                        education = {}
                        for ko_child in ko:
                            if ko_child.tag == 'KoulutuksenNimi':
                                education['title'] = ko_child.text
                            elif ko_child.tag == 'OppilaitoksenNimi':
                                education['organization_name'] = ko_child.text
                        educations.append(education)

            if orcid is not None:

                # Create person object
                person_obj, created = Person.objects.update_or_create(
                    orcid = orcid,
                    first_name = first_name,
                    last_name = last_name,
                    email = email,
                    biography = biography,
                )

                if created:
                    count_new += 1
                else:
                    count_update += 1

                # Create person's keyword objects
                for keyword in keywords:
                    keyword_obj, created = Keyword.objects.update_or_create(
                        person = person_obj,
                        value = keyword,
                    )

                # Create person's link objects
                for link in links:
                    link_obj, created = Link.objects.update_or_create(
                        person = person_obj,
                        url = link["url"],
                        name = link["name"],
                    )

                # Create person's affiliation objects
                for affiliation in affiliations:
                    affiliation_obj, created = Affiliation.objects.update_or_create(
                        person = person_obj,
                        title = affiliation["title"],
                        department_name = affiliation["department_name"],
                    )

                # Create person's education objects
                for education in educations:
                    education_obj, created = Education.objects.update_or_create(
                        person = person_obj,
                        title = education["title"],
                        organization_name = education["organization_name"],
                    )

    print("created " + str(count_new) + ", updated " + str(count_update))