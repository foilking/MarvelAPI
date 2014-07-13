function getView(name) {
    var template = '';
    $.ajax(
            {
                url: '/Template/' + name,
                async: false,
                success: function (text) {
                    template = text;
                }
            });
    return Ember.Handlebars.compile(template);
};

window.MarvelApp = Ember.Application.create();