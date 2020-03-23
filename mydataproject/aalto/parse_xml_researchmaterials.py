# python ../manage.py shell < parse_xml.py

import xml.etree.ElementTree as ET
from aalto.models import *

def parse_researchmaterials(f):
    count_new = 0
    count_update = 0

    for line in f:
        orcid = None
        organizationId = None
        name = None
        description = None
        coverageYearStart = None
        coverageYearEnd = None
        publicationYear = None
        publisherName = None
        doi = None
        links = []
        orgUnits = []
        roles = []

        if line.startswith(b'<Tutkimusaineisto>'):

            try:
                root = ET.fromstring(line)
            except Exception as e:
                print(e)
                continue

            for child in root:
                if child.tag == 'OrganisaatioTunnus':
                    organizationId = child.text
                elif child.tag == 'TutkimusaineistonNimi':
                    name = child.text
                elif child.tag == 'Kuvaus':
                    description = child.text
                elif child.tag == 'JulkaisuVuosi':
                    publicationYear = child.text
                elif child.tag == 'KattavuusAlkuVuosi':
                    coverageYearStart = child.text
                elif child.tag == 'KattavuusLoppuVuosi':
                    coverageYearEnd = child.text
                elif child.tag == 'KustantajanNimi':
                    publisherName = child.text
                elif child.tag == 'DOI':
                    doi = child.text
                elif child.tag == 'Osoitteet':
                    links = []
                    for o in child.findall('OsoiteTeksti'):
                        links.append(o.text)
                elif child.tag == 'OsallistuvatOrgYksikot':
                    orgUnits = []
                    for o in child.findall('YksikkoKoodi'):
                        orgUnits.append(o.text)
                elif child.tag == 'OsallistuvatHenkilot':
                    roles = []
                    for h in child.findall('OsallistuvaHenkilo'):
                        orcidtags = h.findall('ORCID')
                        for ot in orcidtags:
                            orcid = ot.text
                        # Store role only when "OsallistuvaHenkilo" contains "ORCID"
                        if len(orcidtags) > 0:
                            for r in h.findall('Rooli'):
                                roles.append(r.text)

            if orcid is not None:
                try:
                    # Get person object using orcid
                    person = Person.objects.get(orcid=orcid)

                    # Create research material object
                    researchmaterial_obj, created = ResearchMaterial.objects.update_or_create(
                        person = person,
                        organizationId = organizationId,
                        name = name,
                        description = description,
                        coverageYearStart = int(coverageYearStart) if coverageYearStart is not None else None,
                        coverageYearEnd = int(coverageYearEnd) if coverageYearEnd is not None else None,
                        publicationYear = int(publicationYear) if publicationYear is not None else None,
                        publisherName = publisherName,
                        doi = doi,
                        linksCommaSeparated = ",".join(links),
                        orgUnitsCommaSeparated = ",".join(orgUnits),
                        rolesCommaSeparated = ",".join(roles)
                    )
                    if created:
                        count_new += 1
                    else:
                        count_update += 1

                except:
                    pass

    print("created " + str(count_new) + ", updated " + str(count_update))