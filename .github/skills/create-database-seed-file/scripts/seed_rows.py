"""Hand-designed test rows for the tables the Mydata API reads or writes.

Rows are dicts keyed by DDL column name. Columns left out are completed by the
generator (FK -> -1 / NULL, NOT NULL non-FK -> generic value). Ids are explicit so
cross-references stay readable. Values in ILLUSTRATIVE_* are plausible but not
taken from an authoritative code list; they are listed in the plan.
"""

ORCID_DS = 1   # dim_registered_data_source id of ORCID (fixed row)
TTV_DS = 2     # dim_registered_data_source id of Tiedejatutkimus.fi (fixed row)
VIRTA_DS, METAX_DS, FUNDING_DS, HY_DS, AALTO_DS, JYU_DS = 3, 4, 5, 6, 7, 8

PROFILE_API = 'Profile API'
TS = '2025-03-01T09:00:00'

# Same list and order as UserProfileService.GetFieldIdentifiers()
FIELD_IDENTIFIERS = [171, 130, 160, 150, 110, 120, 140, 172, 180, 300, 400, 500, 550, 800, 600, 900]

# field_identifier -> allowed primary link columns in fact_field_values
PRIMARY_LINKS = {
    110: ['dim_name_id'],
    120: ['dim_name_id'],
    130: ['dim_pid_id'],
    140: ['dim_researcher_description_id'],
    150: ['dim_keyword_id'],
    171: ['dim_email_addrress_id'],
    172: ['dim_telephone_number_id'],
    180: ['dim_web_link_id'],
    300: ['dim_affiliation_id'],
    400: ['dim_education_id'],
    500: ['dim_publication_id'],
    550: ['dim_profile_only_publication_id'],
    600: ['dim_research_dataset_id', 'dim_profile_only_dataset_id'],
    800: ['dim_funding_decision_id', 'dim_profile_only_funding_decision_id'],
    900: ['dim_research_activity_id', 'dim_profile_only_research_activity_id'],
}
PRIMARY_LINK_COLUMNS = sorted({c for cols in PRIMARY_LINKS.values() for c in cols}
                              | {'dim_referencedata_field_of_science_id'})
PUT_CODE_FIELDS = [120, 130, 150, 171, 180, 300, 400, 550, 600, 800, 900]
PUT_CODE_REQUIRED_FIELDS = [120, 130, 150, 180, 300, 400, 550, 600, 800, 900]
NEVER_SET_FFV_COLUMNS = ['dim_event_id', 'dim_competence_id', 'dim_research_community_id',
                         'dim_researcher_to_research_community_id', 'dim_referencedata_field_of_science_id']
PROFILE_ONLY = {
    'dim_profile_only_publication': ('dim_profile_only_publication_id', 550),
    'dim_profile_only_dataset': ('dim_profile_only_dataset_id', 600),
    'dim_profile_only_funding_decision': ('dim_profile_only_funding_decision_id', 800),
    'dim_profile_only_research_activity': ('dim_profile_only_research_activity_id', 900),
}

# Referencedata ids used across tables
RD_FUNDING, RD_AWARD, RD_CONF, RD_CONF_PRESENTER, RD_DISTINCTION, RD_MEMBERSHIP, RD_PEER_REVIEW, RD_PEER_REVIEWER = range(1, 9)
RD_SHARING = list(range(9, 17))
RD_USER_CHOICES = [17, 18]
RD_FIELD_OF_SCIENCE = [19, 20, 21]
RD_PUB_TYPE_A1, RD_LANG_EN, RD_LANG_FI, RD_RELATION_TYPE, RD_FIELD_OF_ART = 22, 23, 24, 25, 26

# Pools the generic generator uses for referencedata FKs of generic tables
REF_POOLS = {
    ('fact_dim_referencedata_field_of_science', 'dim_referencedata_id'): RD_FIELD_OF_SCIENCE,
    ('fact_referencedata', 'dim_referencedata_id'): RD_FIELD_OF_SCIENCE,
    ('fact_relation', 'relation_type_code'): [RD_RELATION_TYPE],
    ('fact_dim_referencedata_field_of_art', 'dim_referencedata_id'): [RD_FIELD_OF_ART],
    ('br_artpublication_typecategory', 'dim_referencedataid'): [RD_FIELD_OF_ART],
    ('br_concept_relationship', 'dim_referencedata_id_relationship_type_code'): [RD_RELATION_TYPE],
    ('br_dim_referencedata_dim_call_programme', 'dim_referencedata_id'): RD_FIELD_OF_SCIENCE,
}

ILLUSTRATIVE_CODE_SCHEMES = ['tieteenala2010', 'julkaisutyyppiluokitus', 'kieli', 'relation_type', 'taiteenala']


def _ref(i, scheme, value, fi, en, sv, parent=-1, order=None):
    return dict(id=i, code_scheme=scheme, code_value=value, order=order, state='VALID', name_fi=fi, name_en=en,
                name_sv=sv, source_id='koodistot.suomi.fi', source_description='Reference data',
                dim_referencedataid=parent, created=TS, modified=TS)


def build():
    t = {}

    t['dim_sector'] = [
        dict(id=i, sector_id=str(i), name_fi=fi, name_en=en, name_sv=sv, source_id='Tiedejatutkimus.fi')
        for i, fi, en, sv in [
            (1, 'Yliopisto', 'University', 'Universitet'),
            (2, 'Ammattikorkeakoulu', 'University of applied sciences', 'Yrkeshögskola'),
            (3, 'Tutkimuslaitos', 'Research institute', 'Forskningsinstitut'),
            (4, 'Yliopistollinen sairaala', 'University hospital', 'Universitetssjukhus'),
            (5, 'Muu', 'Other', 'Övrig'),
        ]]

    t['dim_date'] = [
        dict(id=i, year=y, month=m, day=d, source_id=PROFILE_API)
        for i, (y, m, d) in enumerate([
            (2015, 9, 1), (2018, 12, 31), (2019, 1, 1), (2020, 3, 15), (2021, 6, 30),
            (2022, 1, 1), (2023, 5, 10), (2024, 12, 31), (2020, 0, 0), (2023, 0, 0)], start=1)]

    t['dim_geo'] = [
        dict(id=1, country_code='FI', country_id='246', region_id='01', municipality_id='091', country_fi='Suomi',
             country_en='Finland', country_sv='Finland', region_fi='Uusimaa', region_sv='Nyland',
             municipality_fi='Helsinki', municipality_sv='Helsingfors', source_id='Tilastokeskus'),
        dict(id=2, country_code='SE', country_id='752', country_fi='Ruotsi', country_en='Sweden', country_sv='Sverige', source_id='Tilastokeskus'),
        dict(id=3, country_code='DE', country_id='276', country_fi='Saksa', country_en='Germany', country_sv='Tyskland', source_id='Tilastokeskus'),
        dict(id=4, country_code='US', country_id='840', country_fi='Yhdysvallat', country_en='United States', country_sv='Förenta staterna', source_id='Tilastokeskus'),
        dict(id=5, country_code='NO', country_id='578', country_fi='Norja', country_en='Norway', country_sv='Norge', source_id='Tilastokeskus'),
    ]

    # Test organizations; ids 1 and 2 are the fixed ORCID / Tiedejatutkimus.fi rows.
    org = lambda i, fi, en, sv, oid, sector, **kw: dict(
        id=i, name_fi=fi, name_en=en, name_sv=sv, organization_id=oid, dim_sectorid=sector, country_code='FI',
        organization_active=True, data_supplier=kw.pop('supplier', False), dim_registered_data_source_id=TTV_DS,
        source_id='Tiedejatutkimus.fi', source_description='Seed test data', created=TS, modified=TS, **kw)
    t['dim_organization'] = [
        org(3, 'Helsingin yliopisto', 'University of Helsinki', 'Helsingfors universitet', '01901', 1, supplier=True),
        org(4, 'Aalto-yliopisto', 'Aalto University', 'Aalto-universitetet', '10076', 1, supplier=True),
        org(5, 'Jyväskylän yliopisto', 'University of Jyväskylä', 'Jyväskylä universitet', '01906', 1, supplier=True),
        org(6, 'CSC - Tieteen tietotekniikan keskus Oy', 'CSC - IT Center for Science Ltd', 'CSC - IT-centret för vetenskap Ab', None, 5),
        org(7, 'Suomen Akatemia', 'Research Council of Finland', 'Finlands Akademi', None, 5, supplier=True),
        org(8, 'Opetus- ja kulttuuriministeriö', 'Ministry of Education and Culture', 'Undervisnings- och kulturministeriet', '02w52zt87', 5),
        org(9, 'Business Finland', 'Business Finland', 'Business Finland', None, 5),
        org(10, 'Matemaattis-luonnontieteellinen tiedekunta', 'Faculty of Science', 'Matematisk-naturvetenskapliga fakulteten',
            None, 1, dim_organization_broader=3, organization_type='faculty', start_date=1),
    ]

    t['dim_registered_data_source'] = [
        dict(id=i, name=n, dim_organization_id=o, source_id='Tiedejatutkimus.fi', source_description='Seed test data', created=TS, modified=TS)
        for i, n, o in [(VIRTA_DS, 'virta', 6), (METAX_DS, 'metax', 6), (FUNDING_DS, 'sftp_funding', 7),
                        (HY_DS, 'Helsingin yliopisto', 3), (AALTO_DS, 'Aalto-yliopisto', 4), (JYU_DS, 'Jyväskylän yliopisto', 5)]]

    rd = [
        _ref(RD_FUNDING, 'aktiviteetitjaroolit', '13', 'Rahoitus', 'Funding', 'Finansiering'),
        _ref(RD_AWARD, 'aktiviteetitjaroolit', '13.1', 'Palkinto', 'Award', 'Pris', parent=RD_FUNDING),
        _ref(RD_CONF, 'aktiviteetitjaroolit', '3.1', 'Konferenssiesitelmä', 'Conference presentation', 'Konferensföredrag'),
        _ref(RD_CONF_PRESENTER, 'aktiviteetitjaroolit', '3.1.1', 'Esitelmöitsijä', 'Presenter', 'Föredragshållare', parent=RD_CONF),
        _ref(RD_DISTINCTION, 'aktiviteetitjaroolit', '9.2', 'Tunnustus', 'Distinction', 'Utmärkelse'),
        _ref(RD_MEMBERSHIP, 'aktiviteetitjaroolit', '5', 'Jäsenyys', 'Membership', 'Medlemskap'),
        _ref(RD_PEER_REVIEW, 'aktiviteetitjaroolit', '2.3', 'Vertaisarviointi', 'Peer review', 'Referentgranskning'),
        _ref(RD_PEER_REVIEWER, 'aktiviteetitjaroolit', '2.3.1', 'Vertaisarvioija', 'Peer reviewer', 'Referentgranskare', parent=RD_PEER_REVIEW),
    ]
    sharing = [('01', 'Profiilin tiedot', 'Profile information', 'Profilinformation'),
               ('02', 'Sähköpostiosoite', 'Email address', 'E-postadress'),
               ('03', 'Puhelinnumero', 'Phone number', 'Telefonnummer'),
               ('04', 'Affiliaatiot ja koulutus', 'Affiliation and education', 'Anknytningar och utbildning'),
               ('05', 'Julkaisut', 'Publications', 'Publikationer'),
               ('06', 'Tutkimusaineistot', 'Datasets', 'Forskningsdata'),
               ('07', 'Rahoitukset', 'Grants', 'Finansiering'),
               ('08', 'Aktiviteetit ja tunnustukset', 'Activities and distinctions', 'Aktiviteter och utmärkelser')]
    rd += [_ref(i, 'tutkijaprofiilin_luvitus', v, fi, en, sv, order=n)
           for n, (i, (v, fi, en, sv)) in enumerate(zip(RD_SHARING, sharing), start=1)]
    rd += [_ref(17, 'Kiinnostuksen_ilmaiseminen', '1', 'Yhteistyö yritysten kanssa', 'Collaboration with companies', 'Samarbete med företag', order=1),
           _ref(18, 'Kiinnostuksen_ilmaiseminen', '2', 'Mediahaastattelut', 'Media interviews', 'Medieintervjuer', order=2)]
    rd += [_ref(19, 'tieteenala2010', '113', 'Tietojenkäsittely ja informaatiotieteet', 'Computer and information sciences', 'Data- och informationsvetenskap'),
           _ref(20, 'tieteenala2010', '1172', 'Ympäristötiede', 'Environmental sciences', 'Miljövetenskap'),
           _ref(21, 'tieteenala2010', '3111', 'Biolääketieteet', 'Biomedicine', 'Biomedicin'),
           _ref(RD_PUB_TYPE_A1, 'julkaisutyyppiluokitus', 'A1', 'Alkuperäisartikkeli tieteellisessä aikakauslehdessä',
                'Journal article (refereed), original research', 'Originalartikel i vetenskaplig tidskrift'),
           _ref(RD_LANG_EN, 'kieli', 'en', 'englanti', 'English', 'engelska'),
           _ref(RD_LANG_FI, 'kieli', 'fi', 'suomi', 'Finnish', 'finska'),
           _ref(RD_RELATION_TYPE, 'relation_type', 'IsSupplementTo', 'Täydentää', 'Is supplement to', 'Kompletterar'),
           _ref(RD_FIELD_OF_ART, 'taiteenala', '1', 'Arkkitehtuuri', 'Architecture', 'Arkitektur')]
    t['dim_referencedata'] = rd

    # Known persons 1-3 own user profiles; 4-10 are researchers known only from TTV sources.
    orcid = lambda kp: f'0000-0000-0000-{kp:04d}'
    t['dim_known_person'] = [dict(id=kp, source_id=orcid(kp), source_description=PROFILE_API,
                                  dim_registered_data_source_id=ORCID_DS, created=TS, modified=TS) for kp in range(1, 11)]

    people = {1: ('Virtanen', 'Aino'), 2: ('Korhonen', 'Mikko'), 3: ('Nieminen', 'Laura'),
              4: ('Mäkinen', 'Juha'), 5: ('Hämäläinen', 'Sanna')}
    name = lambda i, kp, ds, last, first, full=None, src=PROFILE_API: dict(
        id=i, last_name=last, first_names=first, full_name=full if full else f'{last} {first}',
        dim_known_person_id_confirmed_identity=kp, dim_registered_data_source_id=ds, source_id=src,
        source_description=src, created=TS, modified=TS)
    t['dim_name'] = [
        name(1, 1, ORCID_DS, *people[1]), name(2, 2, ORCID_DS, *people[2]), name(3, 3, ORCID_DS, *people[3]),
        name(4, 1, ORCID_DS, None, None, 'A. Virtanen'), name(5, 2, ORCID_DS, None, None, 'M. J. Korhonen'),
        name(6, 1, VIRTA_DS, *people[1], src='virta'), name(7, 2, VIRTA_DS, *people[2], src='virta'),
        name(8, 3, VIRTA_DS, *people[3], src='virta'), name(9, 4, HY_DS, *people[4], src='Helsingin yliopisto'),
        name(10, 5, FUNDING_DS, *people[5], src='sftp_funding'),
    ]

    pids = [dict(id=kp, pid_content=orcid(kp), pid_type='ORCID', dim_known_person_id=kp, source_id=PROFILE_API,
                 source_description=PROFILE_API, created=TS, modified=TS) for kp in range(1, 11)]
    pids += [dict(id=10 + p, pid_content=f'10.5555/ttv.{2018 + p}.{p:03d}', pid_type='doi', dim_publication_id=p,
                  source_id='virta', source_description='virta', created=TS, modified=TS) for p in range(1, 5)]
    pids.append(dict(id=15, pid_content='57000000001', pid_type='Scopus Author ID', dim_known_person_id=1,
                     source_id=PROFILE_API, source_description=PROFILE_API, created=TS, modified=TS))
    t['dim_pid'] = pids

    t['dim_user_profile'] = [
        dict(id=p, dim_known_person_id=p, profile_username=f'5e5d7a10-0000-4000-8000-00000000000{p}', orcid_id=orcid(p),
             allow_all_subscriptions=False, hidden=False, publish_new_orcid_data=(p == 1), highlight_openess=(p == 2),
             source_id=PROFILE_API, source_description=PROFILE_API, created=TS, modified=TS)
        for p in (1, 2, 3)]

    settings_id = {}
    fds = []
    for p in (1, 2, 3):
        for k, fid in enumerate(FIELD_IDENTIFIERS, start=1):
            sid = (p - 1) * len(FIELD_IDENTIFIERS) + k
            settings_id[(p, fid)] = sid
            fds.append(dict(id=sid, dim_user_profile_id=p, field_identifier=fid, show=fid != 160, source_id=PROFILE_API,
                            source_description=PROFILE_API, created=TS, modified=TS))
    t['dim_field_display_settings'] = fds

    t['dim_user_choices'] = [
        dict(id=n, user_choice_value=(n % 2 == 1), dim_user_profile_id=p, dim_referencedata_id_as_user_choice_label=r,
             source_id=PROFILE_API, source_description=PROFILE_API, created=TS, modified=TS)
        for n, (p, r) in enumerate([(p, r) for p in (1, 2, 3) for r in RD_USER_CHOICES], start=1)]

    t['dim_publication_channel'] = [
        dict(id=i, jufo_code=str(50000 + i), channel_name_anylang=n, publisher_name_text=pub, source_id='JUFO', created=TS, modified=TS)
        for i, n, pub in [(1, 'Journal of Example Computing', 'Example Academic Press'),
                          (2, 'Nordic Journal of Environmental Research', 'Example Academic Press'),
                          (3, 'Annals of Example Biomedicine', 'Example Medical Publishers'),
                          (4, 'Proceedings of the Example Conference on Data Science', 'Example Society'),
                          (5, 'Suomalainen esimerkkiaikakauslehti', 'Esimerkkiseura ry')]]

    pubs = [
        (1, 'Scalable methods for research metadata harmonisation', 2019, 1, 'Virtanen, Aino; Korhonen, Mikko', 2),
        (2, 'Open science practices in Finnish universities', 2020, 5, 'Virtanen, Aino', 1),
        (3, 'Long-term trends in boreal lake temperatures', 2021, 2, 'Korhonen, Mikko; Nieminen, Laura', 2),
        (4, 'Biomarker discovery with federated learning', 2022, 3, 'Nieminen, Laura; Virtanen, Aino', 2),
        (5, 'Persistent identifiers in national research information systems', 2021, 1, 'Virtanen, Aino', 1),
        (6, 'Snow cover modelling in a changing climate', 2018, 2, 'Mäkinen, Juha', 1),
        (7, 'Reproducible pipelines for genomic data', 2023, 3, 'Mäkinen, Juha; Nieminen, Laura', 2),
        (8, 'Data citation in practice', 2023, 4, 'Hämäläinen, Sanna', 1),
        (9, 'Collaborative research data infrastructures', 2024, 4, 'Korhonen, Mikko; Mäkinen, Juha; Virtanen, Aino', 3),
        (10, 'Collaborative research data infrastructures', 2024, 4, 'Korhonen, Mikko; Mäkinen, Juha; Virtanen, Aino', 3),
    ]
    t['dim_publication'] = [
        dict(id=i, dim_publication_id=(9 if i == 10 else None), publication_type_code=RD_PUB_TYPE_A1,
             language_code=(RD_LANG_FI if ch == 5 else RD_LANG_EN), dim_publication_channel_id=ch, jufo_id_frozen=50000 + ch,
             publication_year=y, reporting_year=y, publication_id=f'VIRTA-{y}-{i:04d}',
             publication_org_id=('10076' if i == 10 else '01901'), authors_text=a, number_of_authors=n, publication_name=title,
             journal_name=t['dim_publication_channel'][ch - 1]['channel_name_anylang'],
             doi=(f'10.5555/ttv.{2018 + i}.{i:03d}' if i <= 4 else None), peer_reviewed=True, international_publication=(n > 1),
             source_id='virta', source_description='virta', dim_registered_data_source_id=VIRTA_DS, created=TS, modified=TS)
        for i, title, y, ch, a, n in pubs]

    t['dim_research_data_catalog'] = [
        dict(id=1, source_id='urn:nbn:fi:att:data-catalog-ida', source_description='metax', created=TS, modified=TS),
        dict(id=2, source_id='urn:nbn:fi:att:data-catalog-att', source_description='metax', created=TS, modified=TS),
    ]
    t['dim_research_dataset'] = [
        dict(id=i, dim_research_data_catalog_id=(1 if i % 2 else 2), local_identifier=f'urn:nbn:fi:att:00000000-0000-4000-9000-00000000000{i}',
             version_info=1, dataset_created=f'202{i}-02-01T00:00:00', source_id='metax', source_description='metax',
             dim_registered_data_source_id=METAX_DS, created=TS, modified=TS) for i in range(1, 7)]
    ds_names = ['Boreal lake temperature measurements 1990-2020', 'Research metadata harmonisation benchmark',
                'Federated learning biomarker training data', 'Survey on open science practices',
                'Snow cover observations, Lapland', 'Genomic pipeline test data']
    di = [dict(id=i, dim_research_dataset_id=i, descriptive_item=n, descriptive_item_type='name', descriptive_item_language='en',
               source_id='metax', dim_registered_data_source_id=METAX_DS, created=TS, modified=TS) for i, n in enumerate(ds_names, start=1)]
    di += [dict(id=6 + i, dim_research_dataset_id=i, descriptive_item=f'Description of the dataset: {ds_names[i - 1]}.',
                descriptive_item_type='description', descriptive_item_language='en', source_id='metax',
                dim_registered_data_source_id=METAX_DS, created=TS, modified=TS) for i in range(1, 5)]
    t['dim_descriptive_item'] = di

    t['dim_call_programme'] = [
        dict(id=i, name_fi=fi, name_en=en, name_sv=sv, abbreviation=ab, is_open_call=False, dim_date_id_open=6, dim_date_id_due=7,
             source_id='sftp_funding', dim_registered_data_source_id=FUNDING_DS, created=TS, modified=TS)
        for i, fi, en, sv, ab in [(1, 'Akatemiahanke', 'Academy Project', 'Akademiprojekt', 'AH'),
                                  (2, 'Akatemiatutkijan tehtävä', 'Academy Research Fellow', 'Akademiforskare', 'AT'),
                                  (3, 'Tutkimusinfrastruktuurirahoitus', 'Research infrastructure funding', 'Finansiering av forskningsinfrastruktur', 'FIRI')]]

    fds_rows = [
        (1, 'Data harmonisation for open science', 7, 1, 450000, 1),
        (2, 'Lake ecosystems under climate change', 7, 1, 520000, 2),
        (3, 'Federated biomedicine', 7, 2, 380000, 3),
        (4, 'National research data infrastructure', 7, 3, 1200000, 4),
        (5, 'Industrial data platforms', 9, -1, 250000, 5),
        (6, 'Snow and ice observatory', 7, 1, 300000, 6),
    ]
    t['dim_funding_decision'] = [
        dict(id=i, name_en=n, name_fi=f'{n} (FI)', dim_organization_id_funder=funder, dim_call_programme_id=cp,
             amount_in_EUR=amt, dim_date_id_approval=3 + (i % 3), dim_date_id_start=4 + (i % 3), dim_date_id_end=8,
             dim_name_id_contact_person=(10 if i == 5 else -1), dim_geo_id=1, funder_project_number=f'{340000 + i}',
             acronym=f'PRJ{i}', source_id='sftp_funding', dim_registered_data_source_id=FUNDING_DS, created=TS, modified=TS)
        for i, n, funder, cp, amt, _ in fds_rows]
    t['br_participates_in_funding_group'] = [
        dict(dim_funding_decisionid=f, dim_name_id=n, dim_organization_id=o, role_in_funding_group=r,
             share_of_funding_in_EUR=s, source_id='sftp_funding')
        for f, n, o, r, s in [(1, 6, 3, 'leader', 450000), (2, 7, 3, 'leader', 300000), (2, 8, 4, 'partner', 220000),
                              (3, 8, 4, 'leader', 380000), (4, 6, 3, 'partner', 600000), (5, 10, 9, 'leader', 250000),
                              (6, 9, 3, 'leader', 300000)]]

    t['dim_event'] = [
        dict(id=i, name_en=n, name_fi=n, event_location_text=loc, dim_date_id_start_date=d, dim_date_id_end_date=d,
             dim_geo_id_event_country=g, source_id='Helsingin yliopisto', dim_registered_data_source_id=HY_DS, created=TS, modified=TS)
        for i, n, loc, d, g in [(1, 'Example Conference on Data Science 2023', 'Helsinki', 7, 1),
                                (2, 'Nordic Environmental Research Symposium 2021', 'Stockholm', 5, 2),
                                (3, 'Example Biomedicine Summit 2022', 'Berlin', 6, 3)]]

    t['dim_research_activity'] = [
        dict(id=i, name_en=n, name_fi=f'{n} (FI)', international_collaboration=intl, dim_start_date=sd, dim_end_date=ed,
             dim_country_code=g, dim_event_id=ev, dim_organization_id=o, source_id=src, dim_registered_data_source_id=ds,
             created=TS, modified=TS)
        for i, n, intl, sd, ed, g, ev, o, src, ds in [
            (1, 'Keynote at Example Conference on Data Science', True, 7, 7, 1, 1, 3, 'Helsingin yliopisto', HY_DS),
            (2, 'Reviewer for Nordic Journal of Environmental Research', True, 4, 8, 2, -1, 3, 'Helsingin yliopisto', HY_DS),
            (3, 'Invited talk at Example Biomedicine Summit', True, 6, 6, 3, 3, 4, 'Aalto-yliopisto', AALTO_DS),
            (4, 'Member of the national open science working group', False, 4, -1, 1, -1, 3, 'Helsingin yliopisto', HY_DS),
            (5, 'Data stewardship workshop', False, 5, 5, 1, -1, 5, 'Jyväskylän yliopisto', JYU_DS)]]

    aff = lambda i, kp, o, sd, ed, ds, en, fi=None, sv=None, **kw: dict(
        id=i, dim_known_person_id=kp, dim_organization_id=o, start_date=sd, end_date=ed, position_name_en=en,
        position_name_fi=fi, position_name_sv=sv, dim_registered_data_source_id=ds,
        source_id=PROFILE_API if ds == ORCID_DS else 'Helsingin yliopisto' if ds == HY_DS else 'Aalto-yliopisto',
        source_description=PROFILE_API if ds == ORCID_DS else None, created=TS, modified=TS, **kw)
    t['dim_affiliation'] = [
        aff(1, 1, 3, 3, None, ORCID_DS, 'Postdoctoral Researcher', affiliation_type_fi='', affiliation_type_en='', affiliation_type_sv=''),
        aff(2, 1, -1, 1, 2, ORCID_DS, 'Doctoral Researcher', affiliation_type_fi='', affiliation_type_en='', affiliation_type_sv=''),
        aff(3, 2, 3, 4, None, HY_DS, 'University Lecturer', 'Yliopistonlehtori', 'Universitetslektor', local_identifier='HY-AFF-0003',
            affiliation_type_fi='Henkilökunta', affiliation_type_en='Staff', affiliation_type_sv='Personal'),
        aff(4, 3, 4, 5, None, AALTO_DS, 'Research Scientist', 'Tutkija', 'Forskare', local_identifier='AALTO-AFF-0004'),
        aff(5, 4, 3, 1, None, HY_DS, 'Professor', 'Professori', 'Professor', local_identifier='HY-AFF-0005'),
    ]

    t['dim_identifierless_data'] = [
        dict(id=1, type='organization_unit', value_en='Department of Computer Science', value_fi='', value_sv='', source_id=PROFILE_API),
        dict(id=2, type='organization_name', value_en='Example Research Institute', value_fi='', value_sv='', source_id=PROFILE_API),
        dict(id=3, type='organization_unit', value_en='Department of Physics', value_fi='', value_sv='', dim_identifierless_data_id=2, source_id=PROFILE_API),
        dict(id=4, type='organization_name', value_en='Example Science Foundation', value_fi='', value_sv='', source_id=PROFILE_API),
        dict(id=5, type='organization_name', value_en='Example University Hospital', value_fi='', value_sv='', source_id=PROFILE_API),
        dict(id=6, type='organization_name', value_en='Example Society of Data Science', value_fi='', value_sv='', source_id=PROFILE_API),
        dict(id=7, type='organization_name', value_en='Example Analytics Ltd', value_fi='', value_sv='', source_id=PROFILE_API),
    ]
    for r in t['dim_identifierless_data']:
        r.setdefault('dim_identifierless_data_id', None)
        r.update(dim_organization_id=-1, source_description=PROFILE_API, created=TS, modified=TS)

    t['dim_education'] = [
        dict(id=1, dim_known_person_id=1, name_en='Doctor of Philosophy, Computer Science', degree_granting_institution_name='University of Helsinki',
             dim_start_date=1, dim_end_date=2, dim_registered_data_source_id=ORCID_DS, source_id=PROFILE_API),
        dict(id=2, dim_known_person_id=3, name_en='Master of Science, Biomedicine', degree_granting_institution_name='Aalto University',
             dim_start_date=9, dim_end_date=10, dim_registered_data_source_id=ORCID_DS, source_id=PROFILE_API),
        dict(id=3, dim_known_person_id=4, name_fi='Filosofian tohtori', name_en='Doctor of Philosophy', degree_granting_institution_name='Helsingin yliopisto',
             dim_registered_data_source_id=HY_DS, source_id='Helsingin yliopisto'),
    ]
    t['dim_email_addrress'] = [
        dict(id=1, email='aino.virtanen@example.org', dim_known_person_id=1, dim_registered_data_source_id=ORCID_DS, source_id=PROFILE_API),
        dict(id=2, email='mikko.korhonen@example.org', dim_known_person_id=2, dim_registered_data_source_id=HY_DS, source_id='Helsingin yliopisto'),
        dict(id=3, email='laura.nieminen@example.org', dim_known_person_id=3, dim_registered_data_source_id=ORCID_DS, source_id=PROFILE_API),
        dict(id=4, email='juha.makinen@example.org', dim_known_person_id=4, dim_registered_data_source_id=HY_DS, source_id='Helsingin yliopisto'),
    ]
    t['dim_telephone_number'] = [
        dict(id=1, telephone_number='+358 50 000 0001', dim_known_person_id=1, dim_registered_data_source_id=HY_DS, source_id='Helsingin yliopisto'),
        dict(id=2, telephone_number='+358 50 000 0004', dim_known_person_id=4, dim_registered_data_source_id=HY_DS, source_id='Helsingin yliopisto'),
    ]
    t['dim_keyword'] = [
        dict(id=i, keyword=k, language='en', dim_registered_data_source_id=ds, source_id=src)
        for i, k, ds, src in [(1, 'machine learning', ORCID_DS, PROFILE_API), (2, 'research data management', ORCID_DS, PROFILE_API),
                              (3, 'climate change', ORCID_DS, PROFILE_API), (4, 'open science', VIRTA_DS, 'virta'),
                              (5, 'limnology', VIRTA_DS, 'virta'), (6, 'biomarkers', VIRTA_DS, 'virta')]]
    t['dim_researcher_description'] = [
        dict(id=1, research_description_en='I study research information systems and metadata quality.', dim_known_person_id=1,
             dim_registered_data_source_id=ORCID_DS, source_id=PROFILE_API),
        dict(id=2, research_description_en='Limnologist focusing on boreal lakes.', research_description_fi='Boreaalisten järvien tutkija.',
             dim_known_person_id=2, dim_registered_data_source_id=ORCID_DS, source_id=PROFILE_API),
    ]

    po_common = dict(source_id=PROFILE_API, source_description=PROFILE_API, dim_registered_data_source_id=ORCID_DS, created=TS, modified=TS)
    t['dim_profile_only_publication'] = [
        dict(id=i, dim_known_person_id=kp, publication_name=n, publication_year=y, orcid_work_type=wt, doi_handle=doi,
             publication_id='', authors_text='', **po_common)
        for i, kp, n, y, wt, doi in [(1, 1, 'Metadata quality in practice: a preprint', 2024, 'preprint', '10.5555/orcid.2024.001'),
                                     (2, 2, 'Ice-off dates of Finnish lakes', 2022, 'journal-article', '10.5555/orcid.2022.002'),
                                     (3, 3, 'Teaching federated learning', 2023, 'book-chapter', None)]]
    t['dim_profile_only_dataset'] = [
        dict(id=i, name_en=n, orcid_work_type='data-set', local_identifier='', version_info='', dataset_created=f'{y}-01-15T00:00:00', **po_common)
        for i, n, y in [(1, 'Interview transcripts on metadata practices', 2023), (2, 'Lake ice phenology records', 2021),
                        (3, 'Synthetic biomarker dataset', 2022)]]
    t['dim_profile_only_funding_decision'] = [
        dict(id=i, name_en=n, orcid_work_type=wt, dim_organization_id_funder=funder, amount_in_EUR=amt,
             dim_date_id_start=sd, dim_date_id_end=ed, funder_project_number=num, **po_common)
        for i, n, wt, funder, amt, sd, ed, num in [
            (1, 'Travel grant for metadata research', 'grant', None, 5000, 6, 6, ''),
            (2, 'Young researcher award', 'award', 7, 10000, 5, 5, ''),
            (3, 'Industry contract on federated analytics', 'contract', None, 80000, 6, 8, 'C-2022-01')]]
    t['dim_profile_only_research_activity'] = [
        dict(id=i, name_en=n, orcid_work_type=wt, dim_organization_id=o, dim_date_id_start=sd, dim_date_id_end=ed, **po_common)
        for i, n, wt, o, sd, ed in [(1, 'Best poster award', 'distinction', -1, 7, 7),
                                    (2, 'Reviewer for Annals of Example Biomedicine', 'peer-review', 3, 4, 8),
                                    (3, 'Member of Example Society of Data Science', 'membership', -1, 6, -1)]]

    t['dim_web_link'] = [
        dict(id=1, url='https://example.org/people/aino-virtanen', link_label='Homepage', dim_known_person_id=1),
        dict(id=2, url='https://example.org/people/mikko-korhonen', link_label='Research group', dim_known_person_id=2),
        dict(id=3, url='https://doi.org/10.5555/orcid.ds.001', dim_profile_only_dataset_id=1),
        dict(id=4, url='https://example.org/funding/travel-grant', dim_profile_only_funding_decision_id=1),
        dict(id=5, url='https://example.org/awards/best-poster', dim_profile_only_research_activity_id=1),
        dict(id=6, url='https://example.org/events/data-science-2023', dim_research_activity_id=1,
             dim_registered_data_source_id=HY_DS, source_id='Helsingin yliopisto'),
    ]
    for r in t['dim_web_link']:
        r.setdefault('dim_registered_data_source_id', ORCID_DS)
        r.setdefault('source_id', PROFILE_API)
        r.update(dim_service_id=-1, created=TS, modified=TS)

    # fact_field_values with ORCID put codes created on the fly in dim_pid (ids from 101).
    ffv, put_code = [], [100, 1000000]

    def add(p, fid, col, val, ds, pc=False, idl=-1, role=-1, show=True, primary=False):
        row = dict(dim_user_profile_id=p, dim_field_display_settings_id=settings_id[(p, fid)], dim_registered_data_source_id=ds,
                   dim_identifierless_data_id=idl, dim_referencedata_actor_role_id=role, show=show, primary_value=primary,
                   source_id=PROFILE_API, source_description=PROFILE_API, created=TS, modified=TS)
        row[col] = val
        if pc:
            put_code[0] += 1
            put_code[1] += 1
            pids.append(dict(id=put_code[0], pid_content=str(put_code[1]), pid_type='ORCID put code', dim_known_person_id=p,
                             source_id=PROFILE_API, source_description=PROFILE_API, created=TS, modified=TS))
            row['dim_pid_id_orcid_put_code'] = put_code[0]
        ffv.append(row)

    # Profile 1: ORCID-heavy profile covering every link type
    add(1, 110, 'dim_name_id', 1, ORCID_DS, primary=True)
    add(1, 120, 'dim_name_id', 4, ORCID_DS, pc=True)
    add(1, 130, 'dim_pid_id', 15, ORCID_DS, pc=True)
    add(1, 140, 'dim_researcher_description_id', 1, ORCID_DS)
    add(1, 150, 'dim_keyword_id', 1, ORCID_DS, pc=True)
    add(1, 150, 'dim_keyword_id', 2, ORCID_DS, pc=True)
    add(1, 171, 'dim_email_addrress_id', 1, ORCID_DS, pc=True, primary=True)
    add(1, 172, 'dim_telephone_number_id', 1, HY_DS, show=False)
    add(1, 180, 'dim_web_link_id', 1, ORCID_DS, pc=True)
    add(1, 300, 'dim_affiliation_id', 1, ORCID_DS, pc=True, idl=1, primary=True)
    add(1, 300, 'dim_affiliation_id', 2, ORCID_DS, pc=True, idl=2)
    add(1, 400, 'dim_education_id', 1, ORCID_DS, pc=True)
    add(1, 500, 'dim_publication_id', 1, VIRTA_DS)
    add(1, 500, 'dim_publication_id', 2, VIRTA_DS)
    add(1, 500, 'dim_publication_id', 9, VIRTA_DS)
    add(1, 550, 'dim_profile_only_publication_id', 1, ORCID_DS, pc=True)
    add(1, 600, 'dim_research_dataset_id', 2, METAX_DS)
    add(1, 600, 'dim_profile_only_dataset_id', 1, ORCID_DS, pc=True)
    add(1, 800, 'dim_funding_decision_id', 1, FUNDING_DS)
    add(1, 800, 'dim_profile_only_funding_decision_id', 1, ORCID_DS, pc=True, idl=4, role=RD_FUNDING)
    add(1, 900, 'dim_research_activity_id', 1, HY_DS)
    add(1, 900, 'dim_profile_only_research_activity_id', 1, ORCID_DS, pc=True, idl=5, role=RD_DISTINCTION)
    # Profile 2: mix of TTV-sourced and ORCID items
    add(2, 110, 'dim_name_id', 2, ORCID_DS, primary=True)
    add(2, 120, 'dim_name_id', 5, ORCID_DS, pc=True, show=False)
    add(2, 140, 'dim_researcher_description_id', 2, ORCID_DS)
    add(2, 150, 'dim_keyword_id', 3, ORCID_DS, pc=True)
    add(2, 171, 'dim_email_addrress_id', 2, HY_DS)
    add(2, 180, 'dim_web_link_id', 2, ORCID_DS, pc=True)
    add(2, 300, 'dim_affiliation_id', 3, HY_DS, primary=True)
    add(2, 500, 'dim_publication_id', 3, VIRTA_DS)
    add(2, 550, 'dim_profile_only_publication_id', 2, ORCID_DS, pc=True)
    add(2, 600, 'dim_research_dataset_id', 1, METAX_DS)
    add(2, 600, 'dim_profile_only_dataset_id', 2, ORCID_DS, pc=True)
    add(2, 800, 'dim_funding_decision_id', 2, FUNDING_DS)
    add(2, 800, 'dim_profile_only_funding_decision_id', 2, ORCID_DS, pc=True, role=RD_AWARD)
    add(2, 900, 'dim_research_activity_id', 2, HY_DS)
    add(2, 900, 'dim_profile_only_research_activity_id', 2, ORCID_DS, pc=True, role=RD_PEER_REVIEW)
    # Profile 3: freshly created profile, mostly hidden ORCID items
    add(3, 110, 'dim_name_id', 3, ORCID_DS, primary=True)
    add(3, 171, 'dim_email_addrress_id', 3, ORCID_DS, show=False)
    add(3, 400, 'dim_education_id', 2, ORCID_DS, pc=True, show=False)
    add(3, 500, 'dim_publication_id', 4, VIRTA_DS, show=False)
    add(3, 550, 'dim_profile_only_publication_id', 3, ORCID_DS, pc=True, show=False)
    add(3, 600, 'dim_profile_only_dataset_id', 3, ORCID_DS, pc=True, show=False)
    add(3, 800, 'dim_profile_only_funding_decision_id', 3, ORCID_DS, pc=True, idl=7, role=RD_FUNDING, show=False)
    add(3, 900, 'dim_profile_only_research_activity_id', 3, ORCID_DS, pc=True, idl=6, role=RD_MEMBERSHIP, show=False)
    t['fact_field_values'] = ffv

    fc = lambda ctype, **kw: dict(contribution_type=ctype, source_id='virta', source_description='virta', created=TS, modified=TS, **kw)
    unknown = '<insufficient data>'
    t['fact_contribution'] = [
        fc(unknown, dim_name_id=6, dim_publication_id=1),
        fc(unknown, dim_name_id=7, dim_publication_id=1),
        fc(unknown, dim_name_id=6, dim_publication_id=2),
        fc(unknown, dim_name_id=7, dim_publication_id=3),
        fc(unknown, dim_name_id=8, dim_publication_id=4),
        fc(unknown, dim_name_id=6, dim_publication_id=5),
        fc(unknown, dim_name_id=6, dim_publication_id=10),
        fc(unknown, dim_name_id=7, dim_publication_id=10),
        fc(unknown, dim_name_id=7, dim_research_dataset_id=1),
        fc(unknown, dim_name_id=6, dim_research_dataset_id=2),
        fc(unknown, dim_name_id=8, dim_research_dataset_id=3),
        fc(unknown, dim_name_id=8, dim_research_activity_id=3),
        fc('activity_type', dim_research_activity_id=1, dim_referencedata_actor_role_id=RD_CONF),
        fc('activity_type', dim_research_activity_id=2, dim_referencedata_actor_role_id=RD_PEER_REVIEW),
        fc('researcher_name_activity', dim_name_id=6, dim_research_activity_id=1, dim_referencedata_actor_role_id=RD_CONF_PRESENTER),
        fc('researcher_name_activity', dim_name_id=7, dim_research_activity_id=2, dim_referencedata_actor_role_id=RD_PEER_REVIEWER),
    ]

    t['fact_organization_general'] = [
        dict(id=i, dim_organization_id=o, source_id='Tiedejatutkimus.fi', source_description='Seed test data', created=TS, modified=TS)
        for i, o in [(1, 8), (2, 3), (3, 4), (4, 6)]]
    t['dim_controller_purpose'] = [
        dict(id=n, purpose_code_id=r, consent_controller_id=1, created=TS, modified=TS) for n, r in enumerate(RD_SHARING, start=1)]
    t['fact_profile_data_sharing_consent'] = [
        dict(dim_user_profile_id=p, dim_consent_target_id=c, dim_date_id_consent_granted=g, created=TS, modified=TS)
        for p, c, g in [(1, 1, 7), (1, 2, 7), (1, 4, 7), (1, 5, 7), (1, 8, 7), (2, 1, 8), (2, 5, 8), (3, 1, 8)]]
    return t
