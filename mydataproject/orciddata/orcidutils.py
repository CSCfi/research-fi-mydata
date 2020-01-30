import importlib, requests, json
researchmodels = importlib.import_module("research.models")

def record_json_to_model(researchprofile, orcid_record, datasource_orcid):
    if researchprofile.user.orcid_permission.read_all:
        if orcid_record["activities-summary"]:
            # Distinctions
            # if orcid_record["activities-summary"]["distinctions"]:
            #    researchprofile.activity_distinctions = json.dumps(orcid_record["activities-summary"]["distinctions"]["affiliation-group"])
            # Educations
            if len(orcid_record["activities-summary"]["educations"]["affiliation-group"]) > 0:
                # Create education objects
                for affiliationGroup in orcid_record["activities-summary"]["educations"]["affiliation-group"]:
                    for summary in affiliationGroup['summaries']:
                        educationObj = researchprofile.getPositionObject(summary.get('education-summary', None))
                        if educationObj is not None:
                            educationObj['datasource'] = datasource_orcid
                            try:
                                education = researchmodels.Education(**educationObj)
                                education.save()
                            except Exception as e:
                                print("Exception in orcid_record_json_to_model() educations")
                                print(e)
                                pass

            # Employments
            if len(orcid_record["activities-summary"]["employments"]["affiliation-group"]) > 0:
                # Create employment objects
                for affiliationGroup in orcid_record["activities-summary"]["employments"]["affiliation-group"]:
                    for summary in affiliationGroup['summaries']:
                        employmentObj = researchprofile.getPositionObject(summary.get('employment-summary', None))
                        if employmentObj is not None:
                            employmentObj['datasource'] = datasource_orcid
                            try:
                                employment = researchmodels.Employment(**employmentObj)
                                employment.save()
                            except Exception as e:
                                print("Exception in orcid_record_json_to_model() employments")
                                print(e)
                                pass
            
            # Fundings
            #if orcid_record["activities-summary"]["fundings"]:
            #    researchprofile.activity_fundings = json.dumps(orcid_record["activities-summary"]["fundings"]["group"])
            
            # Invited positions
            #if len(orcid_record["activities-summary"]["invited-positions"]["affiliation-group"]) > 0:
            #    for affiliationGroup in orcid_record["activities-summary"]["invited-positions"]["affiliation-group"]:
            #        for summary in affiliationGroup['summaries']:
            #            invitedPositionObj = researchprofile.getPositionObject(summary.get('invited-position-summary', None))
            #            if invitedPositionObj is not None:
            #                invitedPositionObj['datasource'] = datasource_orcid
            #                try:
            #                    invitedPosition = InvitedPosition(**invitedPositionObj)
            #                    invitedPosition.save()
            #                except Exception as e:
            #                    print("Exception in orcid_record_json_to_model() invited positions")
            #                    print(e)
            #                    pass
                
            # Peer reviews
            #try:
            #    if len(orcid_record["activities-summary"]["peer-reviews"]["group"]) > 0:
            #            for group in orcid_record["activities-summary"]["peer-reviews"]["group"]:
            #                for peerReviewGroup in group["peer-review-group"]:
            #                    for peerReviewSummary in peerReviewGroup["peer-review-summary"]:          
            #                        researchmodels.PeerReview.objects.create(
            #                            researchprofile = researchprofile,
            #                            datasource = datasource_orcid,
            #                            reviewerRole = peerReviewSummary.get('reviewer-role', ''),
            #                            reviewUrl = peerReviewSummary.get('review-url', ''),
            #                            reviewType = peerReviewSummary.get('review-type', ''),
            #                            completionDate = researchprofile.getDate(peerReviewSummary['completion-date'])
            #                    )
            #except Exception as e:
            #    print("Exception in orcid_record_json_to_model() peer reviews")
            #    print(e)
            #    pass

            # Qualifications
            #try:
            #    if orcid_record["activities-summary"]["qualifications"]:
            #        researchprofile.activity_qualifications = json.dumps(orcid_record["activities-summary"]["qualifications"]["affiliation-group"])
            #except Exception as e:
            #    print("Exception in orcid_record_json_to_model() qualifications")
            #    print(e)
            #    pass

            # Research resources
            #try:
            #    if orcid_record["activities-summary"]["research-resources"]:
            #        researchprofile.activity_research_resources = json.dumps(orcid_record["activities-summary"]["research-resources"]["group"])
            #except Exception as e:
            #    print("Exception in orcid_record_json_to_model() research resources")
            #    print(e)
            #    pass

            # Services
            #try:
            #    if orcid_record["activities-summary"]["services"]:
            #        researchprofile.activity_services = json.dumps(orcid_record["activities-summary"]["services"]["affiliation-group"])
            #except Exception as e:
            #    print("Exception in orcid_record_json_to_model() services")
            #    print(e)
            #    pass

            # Works (=Publications)
            try:
                if len(orcid_record["activities-summary"]["works"]["group"]) > 0:
                    # Create publication objects
                    for obj in orcid_record["activities-summary"]["works"]["group"]:
                        # Parse DOI
                        try:
                            doi = None
                            if "external-id" in obj["external-ids"]:
                                for external_id in obj["external-ids"]["external-id"]:
                                    if external_id["external-id-type"] == "doi":
                                        doi = external_id.get("external-id-value", None)
                        except Exception as e:
                            print("Exception in orcid_record_json_to_model() works, when parsing DOI")
                            print(e)
                            pass

                        # Create object
                        name = obj["work-summary"][0]["title"]["title"]["value"]
                        publicationYear = obj["work-summary"][0]["publication-date"]["year"]["value"] if obj["work-summary"][0]["publication-date"] is not None else 0
                        researchprofile.update_or_create_publication(
                            doi,
                            datasource_orcid,
                            name,
                            publicationYear,
                            True
                        )
            except Exception as e:
                print("Exception in orcid_record_json_to_model() works")
                print(e)
                pass

        if orcid_record["person"]:
            # Fist name
            try:
                researchmodels.PersonFirstName.objects.create(
                    researchprofile = researchprofile,
                    datasource = datasource_orcid,
                    includeInProfile = False,
                    value = orcid_record["person"]["name"]["given-names"]["value"]
                )
            except Exception as e:
                print("Exception in orcid_record_json_to_model() first name")
                print(e)
                pass
            
            # Last name
            try:
                researchmodels.PersonLastName.objects.create(
                    researchprofile = researchprofile,
                    datasource = datasource_orcid,
                    includeInProfile = False,
                    value = orcid_record["person"]["name"]["family-name"]["value"]
                )
            except Exception as e:
                print("Exception in orcid_record_json_to_model() last name")
                print(e)
                pass

            # Other names
            if len(orcid_record["person"]["other-names"]["other-name"]) > 0:
                other_names = []
                for obj in orcid_record["person"]["other-names"]["other-name"]:
                    other_names.append(obj["content"])
                if len(other_names) > 0:
                    try:
                        researchmodels.PersonOtherName.objects.create(
                            researchprofile = researchprofile,
                            datasource = datasource_orcid,
                            includeInProfile = False,
                            value = "<br>".join(other_names)
                        )
                    except Exception as e:
                        print("Exception in orcid_record_json_to_model() other names")
                        print(e)
                        pass

            # Biography
            if orcid_record["person"]["biography"] and orcid_record["person"]["biography"]["content"]:
                try:
                    researchmodels.PersonBiography.objects.create(
                        researchprofile = researchprofile,
                        datasource = datasource_orcid,
                        includeInProfile = False,
                        value = orcid_record["person"]["biography"]["content"]
                    )
                except Exception as e:
                    print("Exception in orcid_record_json_to_model() biography")
                    print(e)
                    pass
            # Emails
            if len(orcid_record["person"]["emails"]["email"]) > 0:
                email_list = []
                for obj in orcid_record["person"]["emails"]["email"]:
                    email_list.append(obj['email'])
                if len(email_list) > 0:
                    try:
                        researchmodels.PersonEmail.objects.create(
                            researchprofile = researchprofile,
                            datasource = datasource_orcid,
                            includeInProfile = False,
                            value = "<br>".join(email_list)
                        )
                    except Exception as e:
                        print("Exception in orcid_record_json_to_model() email")
                        print(e)
                        pass
            # Keywords
            if len(orcid_record["person"]["keywords"]["keyword"]) > 0:
                for obj in orcid_record["person"]["keywords"]["keyword"]:
                    try:
                        researchmodels.PersonKeyword.objects.create(
                            researchprofile = researchprofile,
                            datasource = datasource_orcid,
                            includeInProfile = False,
                            value = obj["content"]
                        )
                    except Exception as e:
                        print("Exception in orcid_record_json_to_model() keywords")
                        print(e)
                        pass
            # External identifiers and Researcher URLs to links
            links = []
            if len(orcid_record["person"]["researcher-urls"]["researcher-url"]) > 0:
                for obj in orcid_record["person"]["researcher-urls"]["researcher-url"]:
                    url = obj["url"]["value"]
                    name = obj["url-name"]
                    linkHtml = researchprofile.getLinkHtml(url, name)
                    links.append(linkHtml)
            if len(orcid_record["person"]["external-identifiers"]["external-identifier"]) > 0:
                for obj in orcid_record["person"]["external-identifiers"]["external-identifier"]:
                    url = obj["external-id-url"]["value"]
                    name = obj["external-id-type"]
                    linkHtml = researchprofile.getLinkHtml(url, name)
                    links.append(linkHtml)
            if len(links) > 0:
                try:
                    researchmodels.PersonLink.objects.create(
                        researchprofile = researchprofile,
                        datasource = datasource_orcid,
                        includeInProfile = False,
                        value = "<br>".join(links)
                    )
                except Exception as e:
                    print("Exception in orcid_record_json_to_model() external identifiers")
                    print(e)
                    pass
        researchprofile.save()

def get_data(researchprofile, datasource_orcid):
    social = researchprofile.user.social_auth.get(provider='orcid')
    token = social.extra_data['access_token']

    # Get public data
    headers = {
        'Accept': 'application/json',
        'Authorization type': 'Bearer',
        'Access token': token
    }

    # ORCID API URL
    url = 'https://pub.orcid.org/v3.0/' + researchprofile.get_orcid_id() + '/record'
    print("ORCID RECORD URL = " + url)

    response = requests.get(url, headers=headers)
    print("ORCID http response code " + str(response.status_code))
    if response.status_code == 200:
        try:
            json_data = response.json()
            record_json_to_model(researchprofile, json_data, datasource_orcid)
        except Exception as e:
            print("Exception in get_orcid_data()")
            print(e)
            pass

    return True

def delete_data(researchprofile, datasource_orcid):
    researchprofile.include_orcid_id_in_profile = False

    researchprofile.biographies.filter(datasource=datasource_orcid).delete()
    researchprofile.emails.filter(datasource=datasource_orcid).delete()
    researchprofile.links.filter(datasource=datasource_orcid).delete()
    researchprofile.phones.filter(datasource=datasource_orcid).delete()
    researchprofile.keywords.filter(datasource=datasource_orcid).delete()
    researchprofile.first_names.filter(datasource=datasource_orcid).delete()
    researchprofile.last_names.filter(datasource=datasource_orcid).delete()
    researchprofile.other_names.filter(datasource=datasource_orcid).delete()

    researchmodels.Education.objects.filter(researchprofile = researchprofile).delete()
    researchmodels.Employment.objects.filter(researchprofile = researchprofile).delete()
    researchmodels.Funding.objects.filter(researchprofile = researchprofile).delete()
    researchmodels.InvitedPosition.objects.filter(researchprofile = researchprofile).delete()
    researchmodels.Membership.objects.filter(researchprofile = researchprofile).delete()
    researchmodels.PeerReview.objects.filter(researchprofile = researchprofile).delete()
    researchmodels.Qualifications.objects.filter(researchprofile = researchprofile).delete()
    researchmodels.ResearchResouce.objects.filter(researchprofile = researchprofile).delete()
    researchmodels.Service.objects.filter(researchprofile = researchprofile).delete()

    # Delete publications whose only data source is Orcid.
    # If there are other data sources, keep the publication but remove the Orcid datasource.
    publications = researchmodels.Publication.objects.filter(researchprofile = researchprofile, datasources=datasource_orcid)
    for p in publications:
        if p.datasources.count() == 1:
            p.delete()
        else:
            p.datasources.remove(datasource_orcid)

    researchprofile.save()