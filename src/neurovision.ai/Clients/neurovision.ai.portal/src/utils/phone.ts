export const PHONE_EXAMPLE = "+387 61 123 456";

// E.164: '+' then 8–15 digits, first digit 1–9.
const INTERNATIONAL_PHONE_PATTERN = /^\+[1-9]\d{7,14}$/;

export const normalizeInternationalPhone = (phoneNumber: string): string =>
    phoneNumber.trim().replace(/[\s\-()]/g, "");

export const isValidInternationalPhone = (phoneNumber: string): boolean => {
    const compact = normalizeInternationalPhone(phoneNumber);
    if (!compact) return true;
    return INTERNATIONAL_PHONE_PATTERN.test(compact);
};
