MarvelApp.Router.map(function () {
    this.resource('Comics', function () {
        this.resource('comic', { path: '/:comic_id' }, function () {

        });
    });
});