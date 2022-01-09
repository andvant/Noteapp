let Utils = {
    generateUniqueId,
    dateToLocaleString
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
    let options = { year: 'numeric', month: 'long', day: 'numeric', hour: 'numeric', minute: 'numeric' };
    return new Date(date).toLocaleString({}, options);
}

export default Utils;