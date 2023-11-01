const ViewTypes = { Card: 'card', Table : 'table' };
class Pagination {
    data = [];
    dataUrl = "";
    pageNumber = 0;
    pageSize = 8;
    totalRecords = 0;
    targetElement;

    displayCardCallback;
    displayTableCallback;

    viewType = ViewTypes.Card;

    constructor(element, url, displayCardCallback, displayTableCallback, defaultView = ViewTypes.Card) {
        this.targetElement = element;
        this.dataUrl = url;
        this.fetchData();
        this.displayCardCallback = displayCardCallback;
        this.displayTableCallback = displayTableCallback;
        this.viewType = defaultView;
        if (this.viewType == ViewTypes.Card) {
            $('.card-table').hide();
        }
    }

    fetchData = () => {
        $.ajax({
            type: "POST",
            url: this.dataUrl,
            contentType: 'application/json',
            data: JSON.stringify( {
                pageNumber: this.pageNumber,
                pageSize: this.pageSize
            }),
            success: (data) => {
                this.data = data.data;
                this.pageNumber = data.pageNumber;
                this.totalRecords = data.totalRecords;

                if (this.viewType == ViewTypes.Card) {
                    this.displayCardCallback(this.data);
                }
                else {
                    this.displayTableCallback(this.data);
                }
                this.setupPgButtons();
            },
            error: (data) => {
                alert('An error occured while fetching the data!');
            }
        });
    }

    gotoPage = (pageNumber) => {

        this.pageNumber = pageNumber;
        this.fetchData();
    }

    setupPgButtons = () => {
        const selector = `${this.targetElement} .app-pagination`;
       

        let totalPages = Math.ceil(this.totalRecords / this.pageSize);
        let backDisabled = ``;

        if (this.pageNumber == 0) {
            backDisabled = 'disabled';
        }
        let html = `<button class="pg-btn btn-nav-left" onclick="gotoPage(${this.pageNumber == 0 ? 0 : this.pageNumber - 1})" ${backDisabled}>
                     <i class="fas fa-chevron-left"></i>
                         Previous
                   </button>`;

        $(selector).html('');

        for (let i = 0; i < totalPages; i++) {
            let selected = "";
            if (i == this.pageNumber) {
                selected = "pg-btn-active";
            }
            html +=
                `  <button class="pg-btn ${selected}" onclick="gotoPage(${i})">${i + 1}</button>`;
        }

        let forwardDisabled = '';
        if (this.pageNumber + 1 == totalPages) {
            forwardDisabled = 'disabled';
        }
        // check how many buttons are required.
        html += ` <button class="pg-btn btn-nav-right" ${forwardDisabled} onclick="gotoPage(${this.pageNumber + 1 == totalPages ? totalPages - 1 : this.pageNumber + 1})">
                        Next  

                <i class="fas fa-chevron-right"></i>
            </button>`;

        $(selector).html(html);
    }

    switchView(viewType) {
        this.viewType = viewType;
        if (viewType == ViewTypes.Card) {
            $('.card-table').hide();
        }
        else {
            $('.card-table').show();
        }
        $('.card-table').html('');
        $('.card-list').html('');
        this.fetchData();
    }
}

