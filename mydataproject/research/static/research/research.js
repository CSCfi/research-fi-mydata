// CSRF token for Django api calls.
var csrf_token = null;

// TTV portal api host. Value is set in Django template
var ttv_api_host = null;

var url_publication_include_all = null;
var url_publication_include = null;
var url_publication_add = null;
var url_publication_delete = null;
var url_publication_list = null;

// Datasources
var datasource_ttv = 1;
var datasource_orcid = 2;
var datasource_manual = 3;

// Show busy indicator
function busyIndicatorShow() {
    $('#overlay').show();
}

// Hide busy indicator
function busyIndicatorHide() {
    $('#overlay').hide();
}

// Toggle all publications
function toggleAllPublications() {
    includeAllPublications = !includeAllPublications;

    busyIndicatorShow();

    $.ajax({
        type: 'POST',
        url: url_publication_include_all,
        data: {
            csrfmiddlewaretoken: csrf_token,
            include: includeAllPublications
        },
        success: function() {
            $('.publicationIncludeCheckbox').prop('checked', includeAllPublications);
        },
        dataType: 'json'
    }).done(function() {
        busyIndicatorHide();
    });
}

// Include/exclude publication from research profile
function includePublication(publicationId, oldValue) {
    busyIndicatorShow();

    var include = !oldValue;
    $.ajax({
        type: 'POST',
        url: url_publication_include,
        data: {
            csrfmiddlewaretoken: csrf_token,
            publicationId: publicationId,
            include: include
        },
        success: function() {
        },
        dataType: 'json'
    }).done(function() {
        busyIndicatorHide();
    });
};

// Add publication into research profile
function addPublication(index) {
    busyIndicatorShow();

    var publication = {
        authorsText: publicationSearchResults[index]._source.authorsText,
        publicationName: publicationSearchResults[index]._source.publicationName,
        publicationYear: publicationSearchResults[index]._source.publicationYear,
        doi: publicationSearchResults[index]._source.doi
    };

    $.ajax({
        type: 'POST',
        url: url_publication_add,
        data: {
            csrfmiddlewaretoken: csrf_token,
            publication: JSON.stringify(publication)
        },
        success: function() {
            $('#publicationSearchRow_' + index).remove();
        },
        dataType: 'json'
    }).done(function() {
        busyIndicatorHide();
    });
};

// Delete manually added publication
function deletePublication(publicationId) {
    busyIndicatorShow();

    $.ajax({
        type: 'POST',
        url: url_publication_delete,
        data: {
            csrfmiddlewaretoken: csrf_token,
            publicationId: publicationId
        },
        success: function() {
            $('#publication_' + publicationId).remove();
        },
        dataType: 'json'
    }).done(function() {
        busyIndicatorHide();
    });
};

// TTV publication search results
var publicationSearchResults = [];

var includeAllPublications = false;

// Get publications
function getPublications() {
    busyIndicatorShow();

    $.ajax({
        type: 'POST',
        url: url_publication_list,
        data: { csrfmiddlewaretoken: csrf_token },
        beforeSend: function () {
            // Hide toggle all rows checkbox
            $('#toggleAllPublications').hide();
            // Remove old rows
            $('#publicationTableBody').html('');
        },
        success: function(response) {
            response.publications.forEach(function(p) {
                var checked = p.fields.includeInProfile ? "checked" : "";
                var datasourceIcon = '<i class="fas fa-check"></i>';
                var is_datasource_ttv = (p.fields.datasources.indexOf(datasource_ttv) !== -1);
                var is_datasource_orcid = (p.fields.datasources.indexOf(datasource_orcid) !== -1);
                var is_datasource_manual = (p.fields.datasources.indexOf(datasource_manual) !== -1);
                var doi = p.fields.doi && p.fields.doi !== null ? p.fields.doi : '';
                var publicationElementHtml =
                    '<tr id="publication_' + p.pk + '">' +
                    '<td><input class="publicationIncludeCheckbox" type="checkbox" style="cursor: pointer;" ' + checked + ' onclick="includePublication(' + p.pk + ',' + p.fields.includeInProfile +')"></td>' +
                    '<td>' + (p.fields.publicationYear !== null && p.fields.publicationYear > 0 ? p.fields.publicationYear : '-') + '</td>' +
                    '<td>' + p.fields.name + '</td>' +
                    '<td><a href="https://doi.org/' + doi +'" target="_blank">' + doi + '</a></td>' +
                    '<td>' + (is_datasource_ttv ? datasourceIcon : '') + '</td>' +
                    '<td>' + (is_datasource_orcid ? datasourceIcon : '') + '</td>' +
                    '<td>' + (is_datasource_manual ? datasourceIcon : '') + '</td>';

                    if (is_datasource_manual) {
                        publicationElementHtml += '<td><i class="far fa-trash-alt" onclick="deletePublication(' + p.pk + ')" style="color:#f00;cursor:pointer;"></i></td>';
                    }
                    else {
                        publicationElementHtml += '<td></td>';
                    }
                    publicationElementHtml += '</tr>';
                $('#publicationTableBody').append(publicationElementHtml);
            });

            // Show toggle all publications checkbox
            if (response.publications.length > 1) {
                $('#toggleAllPublications').show();
            }
        },
        dataType: 'json'
    }).done(function() {
        busyIndicatorHide();
    });
};




$(document).ready(function() {
    // Show busy indicator when menu items are selected
    $('.navbar a').click(busyIndicatorShow);

    // Get publications when publications collapse is expanded
    $('#collapsePublications').on('show.bs.collapse', function (e) {
        getPublications();
    });

    // Get publications when publications tab is shown
    $('#publicationTab').on('show.bs.tab', function (e) {
        if (e.target.id === 'pub-list-tab') {
            getPublications();
        }
    });

    // TTV search
    $('#pubTtvSearchBtn').click(function(){
        // Show indicator
        busyIndicatorShow();
    
        // Clear old results
        $('#pubTtvSearchResultTableBody').html('');
    
        // New query
        var query = $('#pubTtvSearchInput').val();
        var search_url = ttv_api_host  + '/portalapi/publication/_search?size=20&q=' + query;
        $.ajax({
            url: search_url,
            success: function(result) {
                console.log("Search result", result);
    
                try {
                    if (result.hits.hits.length > 0) {
                        publicationSearchResults = result.hits.hits;
                        result.hits.hits.forEach(function (publication, index) {
                            var authorsText = publication._source.authorsText;
                            var publicationName = publication._source.publicationName;
                            var publicationYear = publication._source.publicationYear;
                            var doi = publication._source.doi;
                            /*
                            var publicationElementHtml = 
                                '<div class="form-check mb-2"><input class="form-check-input" type="checkbox">' +
                                '<label class="form-check-label small"><strong>' + publicationName + '</strong><br>' + 
                                publicationYear + ' - ' + authorsText  + '</label></div>';
    
                            $('#pubTtvSearchResult').append(publicationElementHtml);
                            */
    
                            var publicationElementHtml =
                                '<tr id="publicationSearchRow_' + index + '">' +
                                '<td>' + publicationYear + '</td>' +
                                '<td>' + publicationName + '</td>' +
                                '<td><a href="https://doi.org/' + doi +'" target="_blank">' + doi + '</a></td>' +
                                '<td>' + authorsText + '</td>' +
                                //'<td><button onclick="addPublication(' + index + ')">+</button></td>' +
                                '<td><i class="fas fa-plus-circle" onclick="addPublication(' + index + ')" style="color:#0a0; cursor:pointer;"></i></td>' +
                                '</tr>';
    
                            $('#pubTtvSearchResultTableBody').append(publicationElementHtml);
                        });
                    } else {
                        publicationSearchResults = [];
                        var publicationElementHtml = '<tr><td colspan="5"><i>Ei hakutuloksia</i></td></tr>';
                        $('#pubTtvSearchResultTableBody').append(publicationElementHtml);
                    }
                } catch (err) { console.error("TTV search result", err); }
            },
            complete: function() {
                // Hide indicator
                busyIndicatorHide();
            }
        });
    });

    // Handle "select all" for person section
    $('#id_person_all').click(function(){
        var person_checked = $('#id_person_all').prop('checked');
        $('.orcid-person').prop('checked', person_checked );
    });

    // Handle "select all" for activities section
    $('#id_activities_all').click(function(){
        var activities_checked = $('#id_activities_all').prop('checked');
        $('.orcid-activities').prop('checked', activities_checked );
    });

    // Show busy indicator when profile create button is pressed.
    $('#createProfileBtn').click(function(){
        $('#overlay').show();
    });
});

