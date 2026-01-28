<!-- src/routes/dashboard/settings/+page.svelte -->
<script lang="ts">
  import { 
    User,
    Bell,
    Lock,
    Mail,
    Globe,
    Moon,
    Sun,
    Save,
    Eye,
    EyeOff,
    Check,
    X
  } from '@lucide/svelte';
  
  import { Button } from '$lib/components/ui/button';
  import { Card, CardContent, CardHeader, CardTitle } from '$lib/components/ui/card';
  import { Input } from '$lib/components/ui/input';
  import { Label } from '$lib/components/ui/label';
  import { Switch } from '$lib/components/ui/switch';
  import { Separator } from '$lib/components/ui/separator';
  import { Tabs, TabsContent, TabsList, TabsTrigger } from '$lib/components/ui/tabs';

  // User profile data
  let profile = {
    name: 'John Doe',
    email: 'john@example.com',
    bio: 'Movie enthusiast and collector',
    location: 'New York, NY',
    joinDate: 'January 2024'
  };

  // Account settings
  let account = {
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
    showCurrentPassword: false,
    showNewPassword: false,
    showConfirmPassword: false
  };

  // Notification preferences
  let notifications = {
    emailUpdates: true,
    newReviews: true,
    movieRecommendations: true,
    weeklyDigest: false,
    marketingEmails: false
  };

  // Privacy settings
  let privacy = {
    profilePublic: true,
    showEmail: false,
    showWatchlist: true,
    showReviews: true,
    showActivity: true
  };

  // UI state
  let activeTab = 'profile';
  let isSaving = false;
  let showDeleteConfirm = false;

  // Theme state (example - would integrate with your theme system)
  let isDarkMode = true;

  const saveProfile = () => {
    isSaving = true;
    // Simulate API call
    setTimeout(() => {
      isSaving = false;
      alert('Profile saved successfully');
    }, 1000);
  };

  const savePassword = () => {
    if (account.newPassword !== account.confirmPassword) {
      alert('New passwords do not match');
      return;
    }

    isSaving = true;
    // Simulate API call
    setTimeout(() => {
      isSaving = false;
      alert('Password updated successfully');
      account = {
        currentPassword: '',
        newPassword: '',
        confirmPassword: '',
        showCurrentPassword: false,
        showNewPassword: false,
        showConfirmPassword: false
      };
    }, 1000);
  };

  const saveNotifications = () => {
    isSaving = true;
    setTimeout(() => {
      isSaving = false;
      alert('Notification preferences saved');
    }, 800);
  };

  const savePrivacy = () => {
    isSaving = true;
    setTimeout(() => {
      isSaving = false;
      alert('Privacy settings saved');
    }, 800);
  };

  const toggleDarkMode = () => {
    isDarkMode = !isDarkMode;
    // In real app, this would trigger your theme system
    alert(`Theme changed to ${isDarkMode ? 'dark' : 'light'} mode`);
  };

  const exportData = () => {
    alert('Data export would start here');
  };

  const deleteAccount = () => {
    if (confirm('Are you sure you want to delete your account? This cannot be undone.')) {
      alert('Account deletion process would start here');
      showDeleteConfirm = false;
    }
  };
</script>

<svelte:head>
  <title>Settings</title>
</svelte:head>

<div class="space-y-6">
  <!-- Header -->
  <div>
    <h1 class="text-2xl font-bold">Settings</h1>
    <p class="text-muted-foreground">Manage your account preferences</p>
  </div>

  <!-- Settings Tabs -->
  <Tabs bind:value={activeTab} class="w-full">
    <TabsList class="grid grid-cols-4 mb-8">
      <TabsTrigger value="profile" class="gap-2">
        <User class="h-4 w-4" />
        Profile
      </TabsTrigger>
      <TabsTrigger value="account" class="gap-2">
        <Lock class="h-4 w-4" />
        Account
      </TabsTrigger>
      <TabsTrigger value="notifications" class="gap-2">
        <Bell class="h-4 w-4" />
        Notifications
      </TabsTrigger>
      <TabsTrigger value="privacy" class="gap-2">
        <Globe class="h-4 w-4" />
        Privacy
      </TabsTrigger>
    </TabsList>

    <!-- PROFILE TAB -->
    <TabsContent value="profile" class="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle>Profile Information</CardTitle>
        </CardHeader>
        <CardContent class="space-y-6">
          <div class="flex items-center gap-6">
            <div class="h-20 w-20 rounded-full bg-primary/10 flex items-center justify-center">
              <User class="h-10 w-10 text-primary" />
            </div>
            <div>
              <h3 class="font-semibold">{profile.name}</h3>
              <p class="text-sm text-muted-foreground">Member since {profile.joinDate}</p>
              <Button variant="outline" size="sm" class="mt-2">
                Change Avatar
              </Button>
            </div>
          </div>

          <Separator />

          <div class="grid gap-4">
            <div class="space-y-2">
              <Label for="name">Display Name</Label>
              <Input 
                id="name" 
                bind:value={profile.name}
                placeholder="Your name"
              />
            </div>

            <div class="space-y-2">
              <Label for="email">Email Address</Label>
              <div class="flex gap-2">
                <Input 
                  id="email" 
                  bind:value={profile.email}
                  type="email"
                  placeholder="your@email.com"
                  class="flex-1"
                />
                <Button variant="outline">
                  <Mail class="h-4 w-4 mr-2" />
                  Verify
                </Button>
              </div>
            </div>

            <div class="space-y-2">
              <Label for="bio">Bio</Label>
              <textarea 
                id="bio" 
                bind:value={profile.bio}
                placeholder="Tell us about yourself..."
                rows="3"
                class="w-full p-2 border rounded-md"
              />
            </div>

            <div class="space-y-2">
              <Label for="location">Location</Label>
              <Input 
                id="location" 
                bind:value={profile.location}
                placeholder="City, Country"
              />
            </div>
          </div>

          <Separator />

          <div class="flex justify-between">
            <div class="flex items-center gap-2">
              <Label>Theme</Label>
              <Button 
                variant="outline" 
                size="sm"
                on:click={toggleDarkMode}
              >
                {#if isDarkMode}
                  <Sun class="h-4 w-4 mr-2" />
                  Switch to Light
                {:else}
                  <Moon class="h-4 w-4 mr-2" />
                  Switch to Dark
                {/if}
              </Button>
            </div>
            
            <Button on:click={saveProfile} disabled={isSaving}>
              {#if isSaving}
                Saving...
              {:else}
                <Save class="h-4 w-4 mr-2" />
                Save Changes
              {/if}
            </Button>
          </div>
        </CardContent>
      </Card>
    </TabsContent>

    <!-- ACCOUNT TAB -->
    <TabsContent value="account" class="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle>Change Password</CardTitle>
        </CardHeader>
        <CardContent class="space-y-4">
          <div class="space-y-2">
            <Label for="currentPassword">Current Password</Label>
            <div class="relative">
              <Input 
                id="currentPassword" 
                bind:value={account.currentPassword}
                type={account.showCurrentPassword ? 'text' : 'password'}
                placeholder="Enter current password"
              />
              <button 
                type="button"
                class="absolute right-3 top-1/2 transform -translate-y-1/2"
                on:click={() => account.showCurrentPassword = !account.showCurrentPassword}
              >
                {#if account.showCurrentPassword}
                  <EyeOff class="h-4 w-4 text-muted-foreground" />
                {:else}
                  <Eye class="h-4 w-4 text-muted-foreground" />
                {/if}
              </button>
            </div>
          </div>

          <div class="space-y-2">
            <Label for="newPassword">New Password</Label>
            <div class="relative">
              <Input 
                id="newPassword" 
                bind:value={account.newPassword}
                type={account.showNewPassword ? 'text' : 'password'}
                placeholder="Enter new password"
              />
              <button 
                type="button"
                class="absolute right-3 top-1/2 transform -translate-y-1/2"
                on:click={() => account.showNewPassword = !account.showNewPassword}
              >
                {#if account.showNewPassword}
                  <EyeOff class="h-4 w-4 text-muted-foreground" />
                {:else}
                  <Eye class="h-4 w-4 text-muted-foreground" />
                {/if}
              </button>
            </div>
          </div>

          <div class="space-y-2">
            <Label for="confirmPassword">Confirm New Password</Label>
            <div class="relative">
              <Input 
                id="confirmPassword" 
                bind:value={account.confirmPassword}
                type={account.showConfirmPassword ? 'text' : 'password'}
                placeholder="Confirm new password"
              />
              <button 
                type="button"
                class="absolute right-3 top-1/2 transform -translate-y-1/2"
                on:click={() => account.showConfirmPassword = !account.showConfirmPassword}
              >
                {#if account.showConfirmPassword}
                  <EyeOff class="h-4 w-4 text-muted-foreground" />
                {:else}
                  <Eye class="h-4 w-4 text-muted-foreground" />
                {/if}
              </button>
            </div>
          </div>

          <div class="pt-4">
            <Button 
              class="w-full" 
              on:click={savePassword}
              disabled={isSaving || !account.currentPassword || !account.newPassword || !account.confirmPassword}
            >
              {#if isSaving}
                Updating...
              {:else}
                <Lock class="h-4 w-4 mr-2" />
                Update Password
              {/if}
            </Button>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Data Management</CardTitle>
        </CardHeader>
        <CardContent class="space-y-4">
          <div class="flex items-center justify-between">
            <div>
              <h3 class="font-medium">Export Your Data</h3>
              <p class="text-sm text-muted-foreground">Download all your movies, reviews, and lists</p>
            </div>
            <Button variant="outline" on:click={exportData}>
              Export Data
            </Button>
          </div>

          <Separator />

          <div class="flex items-center justify-between">
            <div>
              <h3 class="font-medium text-destructive">Delete Account</h3>
              <p class="text-sm text-muted-foreground">Permanently remove your account and all data</p>
            </div>
            <Button 
              variant="destructive" 
              on:click={() => showDeleteConfirm = true}
            >
              Delete Account
            </Button>
          </div>
        </CardContent>
      </Card>
    </TabsContent>

    <!-- NOTIFICATIONS TAB -->
    <TabsContent value="notifications" class="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle>Email Notifications</CardTitle>
        </CardHeader>
        <CardContent class="space-y-4">
          <div class="flex items-center justify-between">
            <div class="space-y-0.5">
              <Label>Email Updates</Label>
              <p class="text-sm text-muted-foreground">General updates about your account</p>
            </div>
            <Switch bind:checked={notifications.emailUpdates} />
          </div>

          <div class="flex items-center justify-between">
            <div class="space-y-0.5">
              <Label>New Reviews</Label>
              <p class="text-sm text-muted-foreground">When someone comments on your reviews</p>
            </div>
            <Switch bind:checked={notifications.newReviews} />
          </div>

          <div class="flex items-center justify-between">
            <div class="space-y-0.5">
              <Label>Movie Recommendations</Label>
              <p class="text-sm text-muted-foreground">Personalized movie suggestions</p>
            </div>
            <Switch bind:checked={notifications.movieRecommendations} />
          </div>

          <div class="flex items-center justify-between">
            <div class="space-y-0.5">
              <Label>Weekly Digest</Label>
              <p class="text-sm text-muted-foreground">Summary of weekly activity</p>
            </div>
            <Switch bind:checked={notifications.weeklyDigest} />
          </div>

          <div class="flex items-center justify-between">
            <div class="space-y-0.5">
              <Label>Marketing Emails</Label>
              <p class="text-sm text-muted-foreground">News and promotions</p>
            </div>
            <Switch bind:checked={notifications.marketingEmails} />
          </div>

          <Separator />

          <div class="pt-4">
            <Button 
              class="w-full" 
              on:click={saveNotifications}
              disabled={isSaving}
            >
              {#if isSaving}
                Saving...
              {:else}
                <Save class="h-4 w-4 mr-2" />
                Save Preferences
              {/if}
            </Button>
          </div>
        </CardContent>
      </Card>
    </TabsContent>

    <!-- PRIVACY TAB -->
    <TabsContent value="privacy" class="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle>Privacy Settings</CardTitle>
        </CardHeader>
        <CardContent class="space-y-4">
          <div class="flex items-center justify-between">
            <div class="space-y-0.5">
              <Label>Public Profile</Label>
              <p class="text-sm text-muted-foreground">Allow others to view your profile</p>
            </div>
            <Switch bind:checked={privacy.profilePublic} />
          </div>

          <div class="flex items-center justify-between">
            <div class="space-y-0.5">
              <Label>Show Email Address</Label>
              <p class="text-sm text-muted-foreground">Display your email on your public profile</p>
            </div>
            <Switch bind:checked={privacy.showEmail} />
          </div>

          <div class="flex items-center justify-between">
            <div class="space-y-0.5">
              <Label>Show Watchlist</Label>
              <p class="text-sm text-muted-foreground">Allow others to see your watchlist</p>
            </div>
            <Switch bind:checked={privacy.showWatchlist} />
          </div>

          <div class="flex items-center justify-between">
            <div class="space-y-0.5">
              <Label>Show Reviews</Label>
              <p class="text-sm text-muted-foreground">Make your reviews visible to others</p>
            </div>
            <Switch bind:checked={privacy.showReviews} />
          </div>

          <div class="flex items-center justify-between">
            <div class="space-y-0.5">
              <Label>Show Activity</Label>
              <p class="text-sm text-muted-foreground">Display your recent activity</p>
            </div>
            <Switch bind:checked={privacy.showActivity} />
          </div>

          <Separator />

          <div class="pt-4">
            <Button 
              class="w-full" 
              on:click={savePrivacy}
              disabled={isSaving}
            >
              {#if isSaving}
                Saving...
              {:else}
                <Save class="h-4 w-4 mr-2" />
                Save Privacy Settings
              {/if}
            </Button>
          </div>
        </CardContent>
      </Card>
    </TabsContent>
  </Tabs>

  <!-- Delete Account Confirmation Modal -->
  {#if showDeleteConfirm}
    <div class="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <Card class="max-w-md w-full mx-4">
        <CardHeader>
          <CardTitle class="text-destructive">Delete Account</CardTitle>
        </CardHeader>
        <CardContent class="space-y-4">
          <p class="text-muted-foreground">
            Are you sure you want to delete your account? This action cannot be undone. All your data including movies, reviews, and lists will be permanently deleted.
          </p>
          
          <div class="flex gap-2 pt-4">
            <Button 
              variant="outline" 
              class="flex-1"
              on:click={() => showDeleteConfirm = false}
            >
              <X class="h-4 w-4 mr-2" />
              Cancel
            </Button>
            <Button 
              variant="destructive" 
              class="flex-1"
              on:click={deleteAccount}
            >
              <Check class="h-4 w-4 mr-2" />
              Delete Account
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  {/if}
</div>