wwtng.controller('TestResources', [
    '$scope', 'dataproxy', '$timeout', '$routeParams', '$http',
    function ($scope, entities, $timeout, $routeParams, $http) {
        $scope.result = '';

        /*$scope.getMyCommunities = function () {
            var req = {
                method: 'GET',
                url: '/Resource/Service/Communities',
                headers: {
                    'LiveUserToken': wwt.user.get('accessToken')
                },
                data: {}
            }//"74864cff8a8e546d40cbcfeed2f119f3"
            //"fe22f7464528b7d3"
            $http(req).
                success(function (data, status, headers, config) {
                    console.log(data);
                    $scope.result = data;
                }).
                error(function (data, status, headers, config) {
                    console.log({ data: data, status: status, headers: headers, config: config });
                });
        };

        var curToken = "EwB4Aq1DBAAUGCCXc8wU/zFu9QnLdZXy+YnElFkAAdrGnaz8vFRvYNraR5HbefJNXL+nB3CNyOprzJAULox3RYoHa2T2daFHqmqcy7Y+RZN4uhd604ez/5GKurAQpMh3Ui7KUQYMxhiFbuzjnEKBmqaS3qbCm9Cijp2YJcjhG5tJ4Hcws8A9H24MU7XY8Ptjk3RZj1ekuF9OxT+LumvtlQGSmGNnc2qURfaOO1jQhFKMJ1oc/HvIUjp1PCphZBmV64/K4oEjVLwPZIAY4Hl+JQ8o4ME70Jzp7vk3CIfgTCirmk1kBk1DD4FUqmz0Qu1XbWCCI0qGCTOHqjUnZdk1ACmyPagXNH23eDLp/GMTshngbEo4k0tGpSVgHg00bHMDZgAACFlGJnSoDYflSAF6RmtmoaY5c1iHa60HZeMa9vKFTsqSEpT1lLfsy/BpEHqwtlkVvMwWLuC0bJbblLVuLrdTXdby/zL3u7k2HZmhEyn2W7XnVDDqJseeOnmLk8lOyOZHC8PyHCrBdqkOCXFcfzEBJ5SQrU7sGfCmmnRneszkBhJ9pjJSpL4UUvNe6uOHuGnyrjfcGEfurpUiVpf9fkNZK9LIXAEkiqEUoNOmK090KD+gbBT8OqJSyYdHP/cryulIacukkXynSuqO/4x+sm2nzYMAXuNNgv37sSi9Z6gYWtabadZcTq6inb6KuuT1LKaXOtw50d4NXnhYnfY3lgo8hltnjTqiwXe/7mYrxNOQvjDv5CuEWJW5ilLp2aqClJlFTwc9Upk1hxsp8UGFZ2EnFi6dlKXZ8bttqwTn6QIlE/6zAOi6oeg+C9t0cn8mndPoTd7XZAE=";

        $scope.getMyCommunitiesExt = function () {
            var req = {
                method: 'GET',
                url: 'http://wwtstaging.azurewebsites.net/Resource/Service/Communities',
                headers: {
                    'LiveUserToken': curToken
                },
                data: {}
            }
            $http(req).
                success(function (data, status, headers, config) {
                    console.log(data);
                    $scope.result = data;
                }).
                error(function (data, status, headers, config) {
                    console.log({ data: data, status: status, headers: headers, config: config });
                });
        };
        $scope.getUser = function () {
            var req = {
                method: 'GET',
                url: 'http://wwtstaging.azurewebsites.net/Resource/Service/User',
                headers: {
                    'LiveUserToken': curToken
                },
                data: {}
            }
            $http(req).
                success(function (data, status, headers, config) {
                    console.log(data);
                    $scope.result = data;
                }).
                error(function (data, status, headers, config) {
                    console.log({ data: data, status: status, headers: headers, config: config });
                });
        };
        $scope.postUser = function () {
            var req = {
                method: 'POST',
                url: 'http://wwtstaging.azurewebsites.net/Resource/Service/User',
                headers: {
                    'LiveUserToken': curToken
                },
                data: {}
            }
            $http(req).
                success(function (data, status, headers, config) {
                    console.log(data);
                    $scope.result = data;
                }).
                error(function (data, status, headers, config) {
                    console.log({ data: data, status: status, headers: headers, config: config });
                });
        };*/
        $scope.getTours = function() {
            $http({
                method: 'GET',
                url: '/Content/Views/tours.xml'
            }).success(function(data) {
                $scope.result = data;
                console.log($(data).find('Tour').length);
                var a = [];
                $(data).find('Tour').each(function (i, tour) {
                    tour = $(tour);
                    a.push([tour.attr('ID'), tour.attr('Title'), tour.parent().attr('Name')]);
                });
                console.log(a);
                parseTourMap(a);
            });
        };
        var tourMap = [
	["01258386-3653-4ccf-9e0f-00bcf22b04d8", "Surface Features of Callisto", "Planets"],
	["c64a7991-b788-439f-bf5f-05dbe2b2c0be", "Lunar Reconnaissance Orbiter", "Planets"],
	["c64a7991-b788-439f-bf5f-05dbe2b2c0be", "Lunar Reconnaissance Orbiter", "Kiosk"],
	["85cacc0f-137f-42e9-a7c1-065f82411dda", "Part 1 Mars surface features", "Planets"],
	["85cacc0f-137f-42e9-a7c1-065f82411dda", "Part 1 Mars surface features", "Mars"],
	["5152b935-0158-494b-b39a-06c8b6c24c1b", "Chinese Valentine - ???", "Other"],
	["95d2fd64-8e59-47f8-8a58-0b88bce9422f", "Beautiful Nebulas", "Nebula"],
	["63c41b85-f27c-4918-9052-0d3ed1a3c377", "Ancient Eye Test", "Star Clusters"],
	["63c41b85-f27c-4918-9052-0d3ed1a3c377", "Ancient Eye Test", "Other"],
	["c5da4211-2ed6-4734-bcd4-16220475635a", "Earth @ Night", "Planets"],
	["c5da4211-2ed6-4734-bcd4-16220475635a", "Earth @ Night", "Kiosk"],
	["c5da4211-2ed6-4734-bcd4-16220475635a", "Earth @ Night", "Other"],
	["c5da4211-2ed6-4734-bcd4-16220475635a", "Earth @ Night", "Earth"],
	["36f62ca3-c45d-452b-aa29-178707c90c17", "IO Surface Tour", "Planets"],
	["d880082b-793a-4287-9ade-190dfc3b378d", "Aurora", "Planets"],
	["d880082b-793a-4287-9ade-190dfc3b378d", "Aurora", "Educators"],
	["d880082b-793a-4287-9ade-190dfc3b378d", "Aurora", "Mars"],
	["d880082b-793a-4287-9ade-190dfc3b378d", "Aurora", "Other"],
	["29dcf26f-1840-4c21-8cb2-1fce711c777f", "Mars Landings", "Planets"],
	["29dcf26f-1840-4c21-8cb2-1fce711c777f", "Mars Landings", "Kiosk"],
	["29dcf26f-1840-4c21-8cb2-1fce711c777f", "Mars Landings", "Mars"],
	["2e5ed67b-9f57-4bad-b630-24c71b3448e1", "WWT | Mars Introductory Tour", "Planets"],
	["2e5ed67b-9f57-4bad-b630-24c71b3448e1", "WWT | Mars Introductory Tour", "Mars"],
	["6f8b1044-467e-4de6-aa0f-24fffaf0d82a", "Adiós Phoenix", "Planets"],
	["6f8b1044-467e-4de6-aa0f-24fffaf0d82a", "Adiós Phoenix", "Kiosk"],
	["6f8b1044-467e-4de6-aa0f-24fffaf0d82a", "Adiós Phoenix", "Mars"],
	["10c8c7a1-bb39-41f7-bdf6-261840c53446", "Bathymetry", "Earth"],
	["56a1ee80-43bb-44d9-8875-28bf1d8ddee4", "Apollo Missions 12 to 14 (Part 2 of 3)", "Planets"],
	["4c729ea7-4d2b-45ed-a5c9-2a77c0424a04", "Aug 1, 2008 Eclipse", "Cosmic Events"],
	["5d5bf20c-57be-4b57-8509-2b6bc7f67c85", "Sombrero Galaxy", "Galaxies"],
	["ebe486e2-6f21-4bc2-b14e-2be6dee5b2c3", "Asteroids", "Planets"],
	["ebe486e2-6f21-4bc2-b14e-2be6dee5b2c3", "Asteroids", "Cosmology"],
	["ebe486e2-6f21-4bc2-b14e-2be6dee5b2c3", "Asteroids", "Mars"],
	["883bb58a-b3e4-4af3-8af4-2ea7534cfe84", "Over the Rainbow: A spectral tour", "Other"],
	["b092903b-a8ef-4ceb-bdfa-32b7dd885837", "Galaxy M81", "Galaxies"],
	["4b3f1c93-b9fb-42f6-82fa-396c56aac95a", "Siberia", "Earth"],
	["ae292f06-adfa-459c-b6be-40199d3fc8aa", "Galileo's New Order", "Planets"],
	["ae292f06-adfa-459c-b6be-40199d3fc8aa", "Galileo's New Order", "Educators"],
	["ae292f06-adfa-459c-b6be-40199d3fc8aa", "Galileo's New Order", "Kiosk"],
	["ae292f06-adfa-459c-b6be-40199d3fc8aa", "Galileo's New Order", "Other"],
	["82ecf2dd-30dd-4a85-bf5d-413f907a1508", "Volcanic Earth", "Earth"],
	["8faeef94-3216-453a-b5f1-4320ce08b8a6", "Phases of the Moon", "Planets"],
	["2819e158-f593-4f7f-a35b-437b0df1a6a2", "WALL-E's Universe", "Kiosk"],
	["d56dea9a-1ad6-4436-bb5d-4463b91e96cd", "Part II: Surface Features of Mars", "Planets"],
	["d56dea9a-1ad6-4436-bb5d-4463b91e96cd", "Part II: Surface Features of Mars", "Mars"],
	["682eb534-8f69-443d-8707-4488d3413c66", "First Pictures of ExtraSolar Planets", "Planets"],
	["682eb534-8f69-443d-8707-4488d3413c66", "First Pictures of ExtraSolar Planets", "Kiosk"],
	["47ba9283-aa46-4e14-a71e-478718e1254a", "Avian Flu", "Earth"],
	["81d4e7d2-fc8e-4674-87e4-4be709695329", "Is There Life on Mars?", "Planets"],
	["81d4e7d2-fc8e-4674-87e4-4be709695329", "Is There Life on Mars?", "Mars"],
	["0d01a583-4ebe-4e7b-b7d6-4ecb630eca38", "WWT Equinox Overview", "Learning WWT"],
	["0d01a583-4ebe-4e7b-b7d6-4ecb630eca38", "WWT Equinox Overview", "Kiosk"],
	["396efe36-4908-4b3d-9a6d-534479f3ef9b", "Using the Observing Time pane", "Learning WWT"],
	["3ff18a4e-1896-44e5-becf-58fec0eef192", "Chandra Crab Nebula", "Nebula"],
	["3ff18a4e-1896-44e5-becf-58fec0eef192", "Chandra Crab Nebula", "Supernova"],
	["6757c90b-ded6-4a6f-a14b-5990024b61d6", "Eclipse China", "Cosmic Events"],
	["6757c90b-ded6-4a6f-a14b-5990024b61d6", "Eclipse China", "Kiosk"],
	["cfe33efa-f1f4-4bd5-9f03-5da1441120ae", "Welcome", "Learning WWT"],
	["99070ea6-6c9f-445c-b610-5e13803f344b", "Interactive Solar Eclipse USA 2017", "Cosmic Events"],
	["41752f4d-53a1-488a-aea4-5fcb076763ad", "HEMSAG", "Planets"],
	["41752f4d-53a1-488a-aea4-5fcb076763ad", "HEMSAG", "Mars"],
	["ef58ddf1-6c4c-4527-82b1-68a390c7149c", "Astronomy for Everyone", "Kiosk"],
	["f2b702da-ec24-4255-9ad4-6aac9c3ad1d2", "Eta Carina Nebula", "Nebula"],
	["f2b702da-ec24-4255-9ad4-6aac9c3ad1d2", "Eta Carina Nebula", "Kiosk"],
	["c1d52cda-9379-48e2-b8f6-6fd973df93cb", "Making a constellation figure", "Learning WWT"],
	["3bf16be6-93d7-48f5-972e-7224cad3d4cb", "Seven top galaxies", "Galaxies"],
	["aa0897c4-4ec3-451e-bc4c-78a8546bb722", "TED", "Surveys"],
	["aa0897c4-4ec3-451e-bc4c-78a8546bb722", "TED", "Other"],
	["9f0d8789-35b8-4eb1-a782-7a5a646d5c8f", "Lunar Reconnaissance Orbiter Camera", "Planets"],
	["ce43c6c4-6839-4752-b78a-7b94dd868935", "The Apollo Program (1961-1975)", "Planets"],
	["ce43c6c4-6839-4752-b78a-7b94dd868935", "The Apollo Program (1961-1975)", "Kiosk"],
	["dae93a0b-1d97-4a25-b67d-7de4febe2920", "Seven Summits", "Earth"],
	["0764072b-4f10-4521-a8d6-7ea86ae960cd", "Gamma-ray Sources", "Surveys"],
	["15ee2668-698a-4a9b-b1a5-817d69de3f63", "Search for Extra Solar Planets", "Planets"],
	["15ee2668-698a-4a9b-b1a5-817d69de3f63", "Search for Extra Solar Planets", "Kiosk"],
	["35673e46-d09c-4c38-a69c-82022accf246", "Apollo Missions 15 to 17 (Part 3 of 3)", "Planets"],
	["48fa32f0-d6d6-47be-bc39-85630a04093a", "Educator's Tour", "Learning WWT"],
	["48fa32f0-d6d6-47be-bc39-85630a04093a", "Educator's Tour", "Educators"],
	["af4a9364-10e9-4b07-a398-90b62ff4c693", "Dust and us", "Nebula"],
	["af4a9364-10e9-4b07-a398-90b62ff4c693", "Dust and us", "Galaxies"],
	["af4a9364-10e9-4b07-a398-90b62ff4c693", "Dust and us", "Black Holes"],
	["af4a9364-10e9-4b07-a398-90b62ff4c693", "Dust and us", "Supernova"],
	["af4a9364-10e9-4b07-a398-90b62ff4c693", "Dust and us", "Kiosk"],
	["f2453b42-a190-4b31-b4ff-91ef9b185e55", "Multiple worlds demo", "Surveys"],
	["f2453b42-a190-4b31-b4ff-91ef9b185e55", "Multiple worlds demo", "Planets"],
	["f2453b42-a190-4b31-b4ff-91ef9b185e55", "Multiple worlds demo", "Kiosk"],
	["2e2aaa42-204b-4f46-b5fa-938ca03dd5c9", "The X-Ray Sky", "Surveys"],
	["2e2aaa42-204b-4f46-b5fa-938ca03dd5c9", "The X-Ray Sky", "Supernova"],
	["2e2aaa42-204b-4f46-b5fa-938ca03dd5c9", "The X-Ray Sky", "Kiosk"],
	["dc85fe26-65ab-4e55-a04e-98ea442875a5", "Binary stars", "Star Clusters"],
	["33319d3e-30df-4d85-95f4-9afcc27127a7", "Pluto", "Planets"],
	["33319d3e-30df-4d85-95f4-9afcc27127a7", "Pluto", "Kiosk"],
	["7f23fcaa-8f44-45b8-9b6e-9c34fbcad7bc", "Venus Surface", "Planets"],
	["cbfc9ca1-4358-45cb-b726-9da0ea804971", "Anglo-Australian Observatory", "Other"],
	["9f9a0f69-94ee-4933-b165-9e58dea0a688", "Mercury Surface Tour", "Planets"],
	["6e408610-2b77-4f79-87fe-a104dfbd547b", "Moons of Mars", "Planets"],
	["6e408610-2b77-4f79-87fe-a104dfbd547b", "Moons of Mars", "Mars"],
	["9122409e-f080-4da6-a2bd-a1c1b5cfe583", "Using the Image Crossfade", "Learning WWT"],
	["9122409e-f080-4da6-a2bd-a1c1b5cfe583", "Using the Image Crossfade", "Planets"],
	["fe4b54d6-1c7c-4217-bd0c-a1e7193deaa3", "M82 Cigar Galaxy", "Galaxies"],
	["fe4b54d6-1c7c-4217-bd0c-a1e7193deaa3", "M82 Cigar Galaxy", "Supernova"],
	["fe4b54d6-1c7c-4217-bd0c-a1e7193deaa3", "M82 Cigar Galaxy", "Kiosk"],
	["ed5fc930-f7d6-4445-826b-a329c9036b0b", "The Eagle Nebula", "Nebula"],
	["ed5fc930-f7d6-4445-826b-a329c9036b0b", "The Eagle Nebula", "Kiosk"],
	["d0f4df1e-0a1c-4624-915f-a340d5e666f9", "Ring of Fire", "Earth"],
	["3ac5d92e-4c11-40b2-b519-a9bda4e18e63", "Orion Nebula - Hubble's Universe", "Nebula"],
	["3ac5d92e-4c11-40b2-b519-a9bda4e18e63", "Orion Nebula - Hubble's Universe", "Kiosk"],
	["e07c51ff-2df1-47f0-9ad6-aad044ef9129", "Create a tour", "Learning WWT"],
	["b425b83d-7c99-47ae-a87e-af6bbc26fd9b", "Dark Matter", "Cosmology"],
	["742673b5-97bf-4eaf-8cf0-b2d1c8d0d8f2", "The Ages of Mars", "Planets"],
	["155461a5-3735-4fe2-89d1-b4504f79abec", "Terapixel", "Surveys"],
	["155461a5-3735-4fe2-89d1-b4504f79abec", "Terapixel", "Mars"],
	["47eee012-af58-4561-b146-bcc44cc92148", "Apollo Intro (Part 1 of 3)", "Planets"],
	["47eee012-af58-4561-b146-bcc44cc92148", "Apollo Intro (Part 1 of 3)", "Kiosk"],
	["3ec29b70-b94c-41c6-9543-be52accbf5ab", "826 Seattle Tour", "Educators"],
	["f44e9460-9469-40b1-a637-bf0a085f93c8", "Ganymede Surface Tour", "Planets"],
	["8251a68d-b8e9-4a19-940b-c061f06692a4", "Opportunity on Mars", "Planets"],
	["8251a68d-b8e9-4a19-940b-c061f06692a4", "Opportunity on Mars", "Kiosk"],
	["8251a68d-b8e9-4a19-940b-c061f06692a4", "Opportunity on Mars", "Mars"],
	["c235e0be-7111-441f-9ae4-c3c6c1fe5b66", "The Ring Nebula", "Nebula"],
	["c235e0be-7111-441f-9ae4-c3c6c1fe5b66", "The Ring Nebula", "Kiosk"],
	["3a794d2a-5f09-40c3-8040-ca6322196c33", "V838 - Light Echo", "Supernova"],
	["12f4ae63-95f7-481c-9801-d135a8b78f14", "Interactive Tours and context", "Learning WWT"],
	["b0bfd463-e072-44cc-8678-d723bea0de6b", "Gas Giants", "Planets"],
	["712bd713-1fa3-44b8-af04-dc2763be17f1", "John Huchra's Universe", "Surveys"],
	["712bd713-1fa3-44b8-af04-dc2763be17f1", "John Huchra's Universe", "Cosmology"],
	["712bd713-1fa3-44b8-af04-dc2763be17f1", "John Huchra's Universe", "Kiosk"],
	["e7921aad-75ab-4edd-a831-e3b88340b1fe", "Space Elevator", "Other"],
	["6f4e5b63-4bce-4622-ad2a-e4b4140dd622", "Seeing the Invisible", "Nebula"],
	["b9268cb1-1f4f-4b40-91bc-e6ea1ba2b6c2", "Naked-eye open star clusters", "Star Clusters"],
	["1f35602f-7606-41c2-aaaa-ea0d7eea064b", "A Brief Trip to Mars", "Planets"],
	["17183f7d-ff6a-47e1-b756-ecd1016175f3", "W5 region", "Nebula"],
	["17183f7d-ff6a-47e1-b756-ecd1016175f3", "W5 region", "Kiosk"],
	["a1ccc18f-bf46-4ccb-9cbe-eef929a86bf4", "Surface Features of Europa", "Planets"],
	["6302a07d-6f5e-48ff-b6cc-ef04185f7760", "Welcome to WWT", "Kiosk"],
	["66cc6d5a-2a73-42e1-9cbd-f613703d15d8", "The First Black Hole", "Black Holes"],
	["66cc6d5a-2a73-42e1-9cbd-f613703d15d8", "The First Black Hole", "Supernova"],
	["c3390ad8-7c45-4881-ae32-f8c099bab588", "Universal Beauty", "Galaxies"],
	["a983e571-1675-44a3-84b2-f965e5ef71f3", "Center of the Milky Way", "Galaxies"],
	["a983e571-1675-44a3-84b2-f965e5ef71f3", "Center of the Milky Way", "Black Holes"],
	["a983e571-1675-44a3-84b2-f965e5ef71f3", "Center of the Milky Way", "Supernova"],
	["b02f4db7-e545-4c34-9951-fd559836846c", "Impact with M31", "Galaxies"],
	["715dd8c4-283c-4dd8-b527-fd6605efe552", "Interesting Objects", "Galaxies"],
	["715dd8c4-283c-4dd8-b527-fd6605efe552", "Interesting Objects", "Surveys"],
	["84e378d7-1320-4923-a525-feca58e50f87", "Fermi", "Surveys"]
];

        var parseTourMap = function (tourMap) {
            tourMap = tourMap.sort(function (a, b) {
                if (a[0] > b[0]) {
                    return 1;
                }
                if (a[0] < b[0]) {
                    return -1;
                }
                return 0;
            });
            var currentGuid;
            var dupCount = 0;
            var arrayCollection = [];
            $.each(tourMap, function(i, item) {

                if (i === 0) {
                    currentGuid = item[0];
                } else if (currentGuid === item[0]) {
                    dupCount++;
                } else {
                    dupCount = 0;
                }
                if (!arrayCollection[dupCount]) {
                    arrayCollection[dupCount] = [];
                }
                arrayCollection[dupCount].push(item);


                currentGuid = item[0];

            });

            var tourMap = '{';
            $.each(arrayCollection, function(i, col) {
                if (i > 0)tourMap += ',';
                tourMap += '\n\tpass' + i + ' :[';
                $.each(col, function(j, item) {
                    if (j > 0)tourMap += ',';
                    tourMap += "'" + item[0] + "'";
                    //tourMap += '\n\t\t["' + item[0] + '","' + item[1] + '","' + item[2] + '"]';
                });
                tourMap += '\n]';
            });

            $scope.result = tourMap + '\n}';
        }
        //parseTourMap(tourMap)
    }]);