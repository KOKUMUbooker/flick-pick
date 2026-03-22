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

// Fetched groups to join 
export interface GroupToJoin {
    id: string,
    name: string
    description: string,
    memberCount: number,
    creatorFullName: string,
    creatorEmail: string,
    createdAt: string
}

export interface FetchGroupsToJoinQueryData {
    userId: string,
    query: string,
    cursor?: string,
    limit?: number,
    direction: CursorDirection
}

export type CursorDirection = "next" | "prev"

export interface FetchGroupsToJoinRes {
    results: GroupToJoin[],
    nextCursor: string,
    prevCursor: string,
    hasNextPage: boolean,
    hasPrevPage: boolean
}

// Fetched users to send invites
export interface FetchedUsersToInviteQueryData {
    groupId: string,
    userId: string,
    query: string,
    cursor?: string,
    limit?: number,
    direction: CursorDirection
}

export interface UserToInvite {
    id: string;
    fullName: string;
    email: string
}

export interface FetchUsersToInviteRes {
    results: UserToInvite[],
    nextCursor: string,
    prevCursor: string,
    hasNextPage: boolean,
    hasPrevPage: boolean
}

// CreateGroupInvitation
export interface CreateInvitationData {
    InviteeUserId: string;
    CreatedById: string;
}

export interface CreateInvitationRes {
    message: string;
    invitationId: string
}


// Update Invitation status
export interface UpdateInvitationData {
    Initiator: string;
    Status: GroupInvitationStatus,
    InvitationId: string,
    GroupId?: string
}

export interface UpdateInvitationRes {
    message: string
}

// Delete invitation
export interface DeleteInvitationData {
    InvitationId: string;
    Initiator: string;
}

export interface DeleteInvitationRes {
    message: string
}