# python ../manage.py shell < parse_xml.py

import xml.etree.ElementTree as ET
from aalto.models import *

def parse_projects(f):
    count_new = 0
    count_update = 0

    for line in f:
        orcid = None
        organizationId = None
        organizationUnits = []
        projectName = None
        projectShortName = None
        projectAbbreviation = None
        projectType = None
        startYear = None
        startMonth = None
        startDay = None
        endYear = None
        endMonth = None
        endDay = None
        role = None

        if line.startswith(b'<Projekti>'):

            try:
                root = ET.fromstring(line)
            except Exception as e:
                print(e)
                continue

            for child in root:
                if child.tag == 'OrganisaatioTunnus':
                    organizationId = child.text
                elif child.tag == 'ProjektinNimi':
                    projectName = child.text
                elif child.tag == 'ProjektinLyhytNimi':
                    projectShortName = child.text
                elif child.tag == 'ProjektinLyhenne':
                    projectAbbreviation = child.text
                elif child.tag == 'ProjektinTyyppi':
                    projectType = child.text
                elif child.tag == 'Alkamisaika':
                    for a_child in child:
                        if a_child.tag == 'Vuosi':
                            startYear = a_child.text
                        elif a_child.tag == 'Kuukausi':
                            startMonth = a_child.text
                        elif a_child.tag == 'Paiva':
                            startDay = a_child.text
                elif child.tag == 'Paattymisaika':
                    for p_child in child:
                        if p_child.tag == 'Vuosi':
                            endYear = p_child.text
                        elif p_child.tag == 'Kuukausi':
                            endMonth = p_child.text
                        elif p_child.tag == 'Paiva':
                            endDay = p_child.text
                elif child.tag == 'OsallistuvatHenkilot':
                    for oh in child.findall('OsallistuvaHenkilo'):
                        for oh_child in oh:
                            if oh_child.tag == 'ORCID':
                                orcid = oh_child.text
                            elif oh_child.tag == 'Rooli':
                                role = oh_child.text
                elif child.tag == 'OsallistuvatOrgYksikot':
                    for orgUnit in child.findall('YksikkoKoodi'):
                        organizationUnits.append(orgUnit.text)

            if orcid is not None:
                try:
                    print(organizationUnits)
                    # Get person object using orcid
                    person = Person.objects.get(orcid=orcid)

                    # Create project object
                    project_obj, created = Project.objects.update_or_create(
                        person = person,
                        organizationId = organizationId,
                        organizationUnitsCommaSeparated = ",".join(organizationUnits),
                        projectName = projectName,
                        projectShortName = projectShortName,
                        projectAbbreviation = projectAbbreviation,
                        projectType = projectType,
                        startYear = int(startYear) if startYear is not None else None,
                        startMonth = int(startMonth) if startMonth is not None else None,
                        startDay = int(startDay) if startDay is not None else None,
                        endYear = int(endYear) if endYear is not None else None,
                        endMonth = int(endMonth) if endMonth is not None else None,
                        endDay = int(endDay) if endDay is not None else None,
                        role = role
                    )
                    if created:
                        count_new += 1
                    else:
                        count_update += 1

                except Exception as e:
                    print(e)
                    pass

    print("created " + str(count_new) + ", updated " + str(count_update))