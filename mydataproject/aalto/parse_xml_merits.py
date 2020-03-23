# python ../manage.py shell < parse_xml.py

import xml.etree.ElementTree as ET
from aalto.models import *

def parse_merits(f):
    count_new = 0
    count_update = 0

    for line in f:
        orcid = None
        organizationId = None
        organizationUnits = []
        meritName = None
        meritType = None
        externalOrganizationName = None
        eventName = None
        eventNumber = None
        journalName = None
        countryCode = None
        cityName = None
        placeName = None
        startYear = None
        startMonth = None
        startDay = None
        endYear = None
        endMonth = None
        endDay = None
        role = None
        url = None

        if line.startswith(b'<Meriitti>'):

            try:
                root = ET.fromstring(line)
            except Exception as e:
                print(e)
                continue

            for child in root:
                if child.tag == 'OrganisaatioTunnus':
                    organizationId = child.text
                elif child.tag == 'OsallistuvatOrgYksikot':
                    for o in child.findall('YksikkoKoodi'):
                        organizationUnits.append(o.text)
                    name = child.text
                elif child.tag == 'MeriitinNimi':
                    meritName = child.text
                elif child.tag == 'MeriitinTyyppi':
                    meritType = child.text
                elif child.tag == 'UlkopuolinenOrganisaatio':
                    externalOrganizationName = child.text
                elif child.tag == 'LehdenNimi':
                    journalName = child.text
                elif child.tag == 'TapahtumanNimi':
                    eventName = child.text
                elif child.tag == 'TapahtumanNumero':
                    eventNumber = child.text
                elif child.tag == 'MaaKoodi':
                    countryCode = child.text
                elif child.tag == 'Kaupunki':
                    cityName = child.text
                elif child.tag == 'Paikka':
                    placeName = child.text
                elif child.tag == 'OsoiteTeksti':
                    url = child.text
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
                        organizationUnits.append(child.text)
            if orcid is not None:
                try:
                    # Get person object using orcid
                    person = Person.objects.get(orcid=orcid)

                    # Create person object
                    merit_obj, created = Merit.objects.update_or_create(
                        person = person,
                        organizationId = organizationId,
                        organizationUnitsCommaSeparated = ",".join(organizationUnits),
                        meritName = meritName,
                        meritType = meritType,
                        externalOrganizationName = externalOrganizationName,
                        eventName = eventName,
                        eventNumber = int(eventNumber) if eventNumber is not None else None,
                        journalName = journalName,
                        countryCode = int(countryCode) if countryCode is not None else None,
                        cityName = cityName,
                        placeName = placeName,
                        url = url,
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