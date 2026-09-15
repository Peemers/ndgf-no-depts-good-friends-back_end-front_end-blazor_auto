//si le user refuse la localisation sur le browser la promise passe en echec
window.getCurrentPosition = () => {
    return new Promise((resolve, reject) => {
        if (!navigator.geolocation) {
            reject("La géolocalisation n'est pas supportée par ce navigateur.");
            return;
        }

        navigator.geolocation.getCurrentPosition(
            (position) => {
                resolve({
                    latitude: position.coords.latitude,
                    longitude: position.coords.longitude
                });
            },
            (error) => {
                reject(error.message);
            }
        );
    });
};