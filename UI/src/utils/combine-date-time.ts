import { CalendarDate, getLocalTimeZone } from "@internationalized/date";

export function combineDateTime(calendarDate: CalendarDate | undefined, time: string) {
    if (!calendarDate || !time) return null;

    const date = calendarDate.toDate(getLocalTimeZone());

    const [hours, minutes] = time.split(":").map(Number);

    date.setHours(hours);
    date.setMinutes(minutes);
    date.setSeconds(0);
    date.setMilliseconds(0);

    return date;
}