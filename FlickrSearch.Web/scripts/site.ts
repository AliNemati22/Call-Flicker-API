$(window).load(function () {
    var map = new Microsoft.Maps.Map('#SDKmap', {
        credentials: 'Ai0_Bu3JzqNWBta84Znq2t7GQoIXEVaaTt90dAKBWj3DjUWFoKGy01jDbotLdMZs',
        center: new Microsoft.Maps.Location(40.776454555, -73.9818155338),
        disableBirdseye: true,
        enableClickableLogo: false,
        showLogo: false
    });

    Microsoft.Maps.Events.addHandler(map, 'click', function (ev: Microsoft.Maps.IMouseEventArgs) {
        $('#txtLat').val(ev.location.latitude);
        $('#txtLon').val(ev.location.longitude);

    });


    let pageIndex = 1;
    var postJSON = function (url, data, callback, errorCallback = undefined) {
        return jQuery.ajax({
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            'type': 'POST',
            'url': url,
            'data': JSON.stringify(data),
            'dataType': 'json',
            'success': callback,
            'error': errorCallback
        });
    };



    $('#btnPre').click(function (event: JQueryEventObject) {
        if (pageIndex == 1)
            return;

        pageIndex--;
        pageIndex = pageIndex <= 0 ? 1 : pageIndex;
        doSearch(pageIndex);
        updatePageIndex(pageIndex);
    });


    $('#btnNext').click(function (event: JQueryEventObject) {
        pageIndex++;
        doSearch(pageIndex);
        updatePageIndex(pageIndex);
    });


    $('#btnSearch').click(function (event: JQueryEventObject) {

        doSearch(1);
        updatePageIndex(1);

    });

    var updatePageIndex = function (i: number) {
        pageIndex = i;
        $('#spnPage').html(pageIndex.toString());
    }

    var doSearch = function (pageIndex: number) {
        var request: ISearchRequestData = {
            Page: pageIndex <= 0 ? 1 : pageIndex,
            Keywords: $('#txtKeywords').val(),
            Lat: $('#txtLat').val(),
            Lon: $('#txtLon').val()
        };


        if (!request.Keywords.trim()) {
            if (!request.Lat || !request.Lon) {
                alert("at Least on of (Keywords/Gelocation info) must be supplied.");
                return;
            }
        }

        $('#overlayDiv').show();
        postJSON('/api/search/findImages', request, function (result: ISearchResultData) {

            $('#overlayDiv').hide();

            if (result.HasError) {
                alert(result.ErrorMessage);
                return;
            }

            $('#spnTotalCount').html(result.Pages.toString());
            $('#resultContainer').empty();

            for (let p of result.Photos) {

                let img = document.createElement("img");
                img.classList.add("r-img");
                img.src = p.ThumbnailUrl;
                img.id = p.id;

                $(img).click(function () {
                    getDetail(p);
                });

                $('#resultContainer').append(img);

            }

        }, function (error: any) {
            $('#overlayDiv').hide();
        });


    }

    var getDetail = function (image: IFlickrSearchPhotoItem) {
        var request: IImageDetailRequest = {
            ImageId: image.id
        };

        $('#detailUrl').attr("href", image.OrginalUrl);
        $('#detailImg').attr("src", image.ThumbnailUrl);
        
        $('#overlayDiv').show();
        postJSON('/api/search/getImageDetail', request, function (result: IImageDetailResponse) {

            $('#overlayDiv').hide();

            if (result.HasError) {
                alert(result.ErrorMessage);
                return;
            }

            $('#detailDesc').empty().html("<b>Title: </b> " + result.Title);
            $('#detailOwner').empty().html("<b>Owner: </b>" + result.Owner);
            $('#detailTags').empty().html("<b>Tags: </b>" + result.Tags.slice(0,7));
            
            
        }, function (error: any) {
            $('#overlayDiv').hide();
        });


    }

});



interface ISearchRequestData {
    Page: number;
    Keywords?: string;
    Lat?: number;
    Lon?: number;
}

interface IImageDetailRequest {
    ImageId: string;
}

interface IBaseResponse {
    HasError: boolean;
    ErrorMessage: string;

}

interface ISearchResultData extends IBaseResponse {

    Photos: IFlickrSearchPhotoItem[];

    Pages: number;
    PerPage: number;
    Total: number;
}


interface IFlickrSearchPhotoItem {

    id: string;
    owner: string;
    secret: string;
    server: string;
    farm: string;
    title: string;
    OrginalUrl: string;
    ThumbnailUrl: string;
}

interface IImageDetailResponse extends IBaseResponse {
    Owner: string;
    Title: string;
    Tags: string[];
    Dates: string[];
    Urls: string[];
    ThumbnailUrl: string;
}