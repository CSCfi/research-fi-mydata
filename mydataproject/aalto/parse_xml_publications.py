# python ../manage.py shell < parse_xml.py

import xml.etree.ElementTree as ET
from aalto.models import Publication

def parse_publications(f):
    count_new = 0
    count_update = 0

    for line in f:
        orcid = None
        name = None
        publicationYear = None
        doi = None

        if line.startswith(b'<Julkaisu>'):

            try:
                root = ET.fromstring(line)
            except Exception as e:
                print(e)
                continue

            for child in root:
                if child.tag == 'ORCID':
                    orcid = child.text
                elif child.tag == 'JulkaisunNimi':
                    name = child.text
                elif child.tag == 'JulkaisuVuosi':
                    publicationYear = child.text
                elif child.tag == 'DOI':
                    doi = child.text
                
            if orcid is not None:
                print(orcid + " " + publicationYear + " " + doi + " " + name)
                # # Create publication object
                # person_obj, created = Publication.objects.update_or_create(
                #     orcid = orcid,
                #     name = name,
                #     publicationYear = publicationYear,
                #     doi = doi,
                # )

                if created:
                    count_new += 1
                else:
                    count_update += 1             

    print("created " + str(count_new) + ", updated " + str(count_update))