export interface GroupInvitationsRes {
    invitations: GroupInvitationItem[]
}

export interface GroupInvitationItem {
    id: string,
    group: {
        name: string,
        description: string,
    },
    invitee: {
        fullName: string,
        email: string
    },
    createdBy: {
        fullName: string,
        email: string
    },
    status: GroupInvitationStatus
}

export type GroupInvitationStatus = "pending" | "approved" | "cancelled"