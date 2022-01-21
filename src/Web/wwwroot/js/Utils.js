let Utils = {
    generateUniqueId,
    dateToLocaleString,
    delay
}

function generateUniqueId()
{
    const ALPHABET = 'abcdef0123456789';
    const ELEMENT_ID_LENGTH = 8;

    let elementId = '';
    for (let i = 0; i < ELEMENT_ID_LENGTH; i++)
    {
        let index = Math.floor((Math.random() * 1_000_000_000) % ALPHABET.length)
        elementId += ALPHABET[index];
    }

    return elementId;
}

function dateToLocaleString(date) {
    let datetime = new Date(date);
    if (datetime instanceof Date && !isNaN(datetime)) {
        let options = { year: 'numeric', month: 'long', day: 'numeric', hour: 'numeric', minute: 'numeric' };
        return datetime.toLocaleString({}, options);
    }
    return "-";
}

function delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

export default Utils;