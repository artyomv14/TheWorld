//tripsControllers.js

(function () {

    "use strict";

    //Getting the exisiting module
    angular.module("app-trips")
        .controller("tripsController", tripsController);

    function tripsController($http) {

        var viewModel = this;
        viewModel.trips = [];

        viewModel.newTrip = {};

        viewModel.errorMessage = "";
        viewModel.isBusy = true;

        $http.get("/api/trips")
            .then(function (response) {
                //Succeed
                angular.copy(response.data, viewModel.trips);
            },
                function (error) {
                    //Fail
                    viewModel.errorMessage = "Failed to load data: " + error;
                })
                .finally(function () {
                    viewModel.isBusy = false;
                });

        viewModel.addTrip = function () {
            viewModel.isBusy = true;
            $http.post("/api/trips", viewModel.newTrip)
                .then(function (response) {
                    //Succeed
                    viewModel.trips.push(response.data);
                    viewModel.newTrip = {};
                },
                    function () {
                        //Fail
                        viewModel.errorMessage = "Failed to save new trip";
                    })
                .finally(function () {
                    viewModel.isBusy = false;
                });
        };
    }

})();