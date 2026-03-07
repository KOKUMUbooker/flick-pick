import type { DBGroup } from "../../types";

export interface AddGroupData {
    Name: string;
    UserId: string;
    Description?: string
}

export interface AddGroupRes {
    message: string;
    group: DBGroup;
}