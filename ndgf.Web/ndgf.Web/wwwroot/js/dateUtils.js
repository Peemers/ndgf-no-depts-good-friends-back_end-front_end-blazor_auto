window.formatAllLocalTimes = () => {
    const elements = document.querySelectorAll('.local-time');
    console.log(`formatAllLocalTimes: ${elements.length} éléments trouvés`);
    elements.forEach(el => {
        const rawDate = el.getAttribute('datetime');
        const date = new Date(rawDate);
        console.log(`Raw: ${rawDate}, Parsed: ${date}, Valid: ${!isNaN(date)}`);
        el.textContent = date.toLocaleString('fr-FR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    });
};