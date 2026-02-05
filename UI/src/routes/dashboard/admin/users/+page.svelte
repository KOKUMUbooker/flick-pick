<!-- src/routes/dashboard/admin-users/+page.svelte -->
<script lang="ts">
	import {
		Calendar,
		Eye,
		Film,
		Mail,
		MoreVertical,
		Search,
		Shield,
		User,
		Users,
		XCircle
	} from '@lucide/svelte';

	import { Badge } from '$lib/components/ui/badge';
	import { Button } from '$lib/components/ui/button';
	import { Card, CardContent, CardHeader, CardTitle } from '$lib/components/ui/card';
	import { Input } from '$lib/components/ui/input';
	import { Separator } from '$lib/components/ui/separator';
	import { RoleEnum, type User as UserModel } from '../../../../types';

	// Sample users data
	let users: UserModel[] = [
		{
			id: 1,
			name: 'John Doe',
			email: 'john@example.com',
			role: RoleEnum.User,
			joinDate: '2024-01-15',
			movies: 2
		},
		{
			id: 2,
			name: 'Sarah Chen',
			email: 'sarah@example.com',
			role: RoleEnum.User,
			joinDate: '2024-01-10',
			movies: 2
		},
		{
			id: 3,
			name: 'Mike Ross',
			email: 'mike@example.com',
			role: RoleEnum.User,
			joinDate: '2024-01-05',
			movies: 2
		},
		{
			id: 4,
			name: 'Admin User',
			email: 'admin@example.com',
			role: RoleEnum.Admin,
			joinDate: '2024-01-01',
			movies: 2
		},
		{
			id: 5,
			name: 'Emma Wilson',
			email: 'emma@example.com',
			role: RoleEnum.User,
			joinDate: '2023-12-20',
			movies: 2
		},
		{
			id: 6,
			name: 'David Lee',
			email: 'david@example.com',
			role: RoleEnum.User,
			joinDate: '2023-12-15',
			movies: 2
		},
		{
			id: 7,
			name: 'Lisa Brown',
			email: 'lisa@example.com',
			role: RoleEnum.User,
			joinDate: '2023-12-10',
			movies: 2
		},
		{
			id: 8,
			name: 'Alex Garcia',
			email: 'alex@example.com',
			role: RoleEnum.User,
			joinDate: '2023-12-01',
			movies: 2
		}
	];

	// State
	let searchQuery = '';
	let filterRole: RoleEnum | null = null; // all, user, admin
	let filterStatus = 'all'; // all, active, suspended, inactive
	let selectedUser: UserModel | null;

	// Filtered users
	$: filteredUsers = users.filter((user) => {
		const matchesSearch =
			!searchQuery ||
			user.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
			user.email.toLowerCase().includes(searchQuery.toLowerCase());

		const matchesRole = user.role === filterRole;

		return matchesSearch && matchesRole;
	});

	// Stats
	$: stats = {
		total: users.length,
		admins: users.filter((u) => u.role === RoleEnum.Admin).length,
		users: users.filter((u) => u.role === RoleEnum.User).length
	};

	const toggleUserRole = (user: UserModel | null) => {
		if (!user) return;
		const userId = user.id;
		users = users.map((user) =>
			user.id === userId
				? {
						...user,
						lastActive: 'Just now (role changed)'
					}
				: user
		);
	};

	const deleteUser = (userId: number) => {
		if (confirm('Are you sure you want to delete this user? This cannot be undone.')) {
			users = users.filter((user) => user.id !== userId);
			selectedUser = null;
		}
	};

	const viewUserDetails = (user: any) => {
		selectedUser = user;
	};

	const closeDetails = () => {
		selectedUser = null;
	};
</script>

<svelte:head>
	<title>Admin - User Management</title>
</svelte:head>

<div class="space-y-6">
	<!-- Header -->
	<div class="flex items-center justify-between">
		<div>
			<h1 class="text-2xl font-bold">User Management</h1>
			<p class="text-muted-foreground">Manage user accounts and permissions</p>
		</div>
		<Button>
			<User class="mr-2 h-4 w-4" />
			Add User
		</Button>
	</div>

	<!-- Stats -->
	<div class="grid grid-cols-2 gap-4 md:grid-cols-3">
		<Card>
			<CardContent class="pt-6 text-center">
				<div class="text-2xl font-bold">{stats.total}</div>
				<p class="text-sm text-muted-foreground">Total Users</p>
			</CardContent>
		</Card>
		<Card>
			<CardContent class="pt-6 text-center">
				<div class="text-2xl font-bold text-purple-500">{stats.admins}</div>
				<p class="text-sm text-muted-foreground">Admins</p>
			</CardContent>
		</Card>
		<Card>
			<CardContent class="pt-6 text-center">
				<div class="text-2xl font-bold text-blue-500">{stats.users}</div>
				<p class="text-sm text-muted-foreground">Regular Users</p>
			</CardContent>
		</Card>
	</div>

	<!-- Search and Filter -->
	<Card>
		<CardContent class="pt-6">
			<div class="space-y-4">
				<div class="relative">
					<Search
						class="absolute top-1/2 left-3 h-4 w-4 -translate-y-1/2 transform text-muted-foreground"
					/>
					<Input
						bind:value={searchQuery}
						placeholder="Search users by name or email..."
						class="pl-10"
					/>
				</div>

				<div class="flex flex-wrap gap-2">
					<!-- Role Filters -->
					<div class="flex gap-2">
						<span class="self-center text-sm text-muted-foreground">Role:</span>
						<Button
							variant={filterRole === null ? 'default' : 'outline'}
							size="sm"
							onclick={() => (filterRole = null)}
						>
							All
						</Button>
						<Button
							variant={filterRole === RoleEnum.User ? 'default' : 'outline'}
							size="sm"
							onclick={() => (filterRole = RoleEnum.User)}
						>
							Users ({stats.users})
						</Button>
						<Button
							variant={filterRole === RoleEnum.Admin ? 'default' : 'outline'}
							size="sm"
							onclick={() => (filterRole = RoleEnum.Admin)}
						>
							Admins ({stats.admins})
						</Button>
					</div>

					<!-- Status Filters -->
					<div class="flex gap-2">
						<span class="ml-4 self-center text-sm text-muted-foreground">Status:</span>
						<Button
							variant={filterStatus === 'all' ? 'default' : 'outline'}
							size="sm"
							onclick={() => (filterStatus = 'all')}
						>
							All
						</Button>
					</div>
				</div>
			</div>
		</CardContent>
	</Card>

	<!-- Users List -->
	<div class="space-y-4">
		{#if filteredUsers.length === 0}
			<Card>
				<CardContent class="py-12 text-center">
					<Users class="mx-auto mb-4 h-12 w-12 text-muted-foreground opacity-50" />
					<h3 class="mb-2 text-lg font-semibold">No users found</h3>
					<p class="text-muted-foreground">
						{searchQuery ? 'Try a different search' : 'No users match the current filters'}
					</p>
				</CardContent>
			</Card>
		{:else}
			{#each filteredUsers as user}
				<Card class="transition-colors hover:bg-accent/50">
					<CardContent class="p-6">
						<div class="flex items-center gap-4">
							<!-- User Avatar -->
							<div class="flex h-12 w-12 items-center justify-center rounded-full bg-primary/10">
								<User class="h-6 w-6 text-primary" />
							</div>

							<!-- User Info -->
							<div class="flex-1">
								<div class="mb-2 flex items-start justify-between">
									<div>
										<div class="mb-1 flex items-center gap-2">
											<h3 class="font-semibold">{user.name}</h3>
											<Badge
												variant={user.role === RoleEnum.Admin ? 'default' : 'outline'}
												class={user.role === RoleEnum.Admin ? 'bg-purple-500' : ''}
											>
												{user.role === RoleEnum.Admin ? 'Admin' : 'User'}
											</Badge>
										</div>

										<div class="flex items-center gap-3 text-sm text-muted-foreground">
											<div class="flex items-center gap-1">
												<Mail class="h-3 w-3" />
												<span>{user.email}</span>
											</div>
											<div class="flex items-center gap-1">
												<Calendar class="h-3 w-3" />
												<span>Joined {user.joinDate}</span>
											</div>
										</div>
									</div>

									<div class="flex items-center gap-1">
										<Button variant="ghost" size="sm" onclick={() => viewUserDetails(user)}>
											<Eye class="h-4 w-4" />
										</Button>
										<Button variant="ghost" size="sm">
											<MoreVertical class="h-4 w-4" />
										</Button>
									</div>
								</div>

								<!-- User Stats -->
								<div class="flex items-center gap-4 text-sm">
									<div class="flex items-center gap-1">
										<Film class="h-4 w-4 text-blue-500" />
										<span>{user.movies} movies</span>
									</div>
								</div>

								<!-- Actions -->
								<div class="mt-4 flex gap-2">
									{#if user.role === RoleEnum.Admin}
										<Button
											variant="outline"
											size="sm"
											class="gap-2"
											onclick={() => toggleUserRole(user)}
										>
											<Shield class="h-4 w-4" />
											Remove Admin
										</Button>
									{:else}
										<Button
											variant="outline"
											size="sm"
											class="gap-2"
											onclick={() => toggleUserRole(user)}
										>
											<Shield class="h-4 w-4" />
											Make Admin
										</Button>
									{/if}

									<Button
										variant="destructive"
										size="sm"
										class="gap-2"
										onclick={() => deleteUser(user.id)}
									>
										<XCircle class="h-4 w-4" />
										Delete
									</Button>
								</div>
							</div>
						</div>
					</CardContent>
				</Card>
			{/each}
		{/if}
	</div>
</div>

<!-- User Details Modal -->
{#if selectedUser}
	<div class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
		<Card class="w-full max-w-md">
			<CardHeader>
				<div class="flex items-start justify-between">
					<CardTitle>User Details</CardTitle>
					<Button variant="ghost" size="sm" onclick={closeDetails}>Close</Button>
				</div>
			</CardHeader>
			<CardContent class="space-y-6">
				<!-- User Info -->
				<div class="flex items-center gap-4">
					<div class="flex h-16 w-16 items-center justify-center rounded-full bg-primary/10">
						<User class="h-8 w-8 text-primary" />
					</div>
					<div>
						<h3 class="text-lg font-semibold">{selectedUser.name}</h3>
						<p class="text-muted-foreground">{selectedUser.email}</p>
					</div>
				</div>

				<!-- Details Grid -->
				<div class="grid grid-cols-2 gap-4">
					<div>
						<h4 class="text-sm font-medium text-muted-foreground">Role</h4>
						<Badge
							variant={selectedUser.role === RoleEnum.Admin ? 'default' : 'outline'}
							class="mt-1"
						>
							{selectedUser.role === RoleEnum.Admin ? 'Administrator' : 'Regular User'}
						</Badge>
					</div>
					<div>
						<h4 class="text-sm font-medium text-muted-foreground">Join Date</h4>
						<p class="mt-1">{selectedUser.joinDate}</p>
					</div>
					<div>
						<h4 class="text-sm font-medium text-muted-foreground">Movies Added</h4>
						<p class="mt-1">{selectedUser.movies} movies</p>
					</div>
				</div>

				<!-- Quick Actions -->
				<Separator />

				<div class="space-y-3">
					<h4 class="font-medium">Quick Actions</h4>

					<div class="grid grid-cols-2 gap-2">
						{#if selectedUser.role === RoleEnum.Admin}
							<Button
								variant="outline"
								onclick={() => {
									toggleUserRole(selectedUser);
									closeDetails();
								}}
							>
								<Shield class="mr-2 h-4 w-4" />
								Remove Admin
							</Button>
						{:else}
							<Button
								variant="outline"
								onclick={() => {
									if (selectedUser) return;
									toggleUserRole(selectedUser);
									closeDetails();
								}}
							>
								<Shield class="mr-2 h-4 w-4" />
								Make Admin
							</Button>
						{/if}
					</div>

					<Button
						variant="destructive"
						class="w-full"
						onclick={() => {
							deleteUser(selectedUser?.id || -1);
							closeDetails();
						}}
					>
						<XCircle class="mr-2 h-4 w-4" />
						Delete User Account
					</Button>
				</div>
			</CardContent>
		</Card>
	</div>
{/if}
