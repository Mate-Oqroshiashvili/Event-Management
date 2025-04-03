import { Routes } from '@angular/router';
import { HomeComponent } from './get-request-components/home/home.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { AdminPanelComponent } from './get-request-components/admin-panel/admin-panel.component';
import { AvailablePromoCodesComponent } from './get-request-components/available-promo-codes/available-promo-codes.component';
import { EventsComponent } from './get-request-components/events/events.component';
import { EventPageComponent } from './get-request-components/event-page/event-page.component';
import { LocationsComponent } from './get-request-components/locations/locations.component';
import { LocationPageComponent } from './get-request-components/location-page/location-page.component';
import { RegisterComponent } from './post-request-components/register/register.component';
import { LoginComponent } from './post-request-components/login/login.component';
import { OrganizersComponent } from './get-request-components/organizers/organizers.component';
import { OrganizerPageComponent } from './get-request-components/organizer-page/organizer-page.component';
import { OrganizerPanelComponent } from './get-request-components/organizer-panel/organizer-panel.component';
import { ProfileComponent } from './get-request-components/profile/profile.component';
import { UserInfoComponent } from './get-request-components/user-info/user-info.component';
import { UserAnalyticsComponent } from './get-request-components/user-analytics/user-analytics.component';
import { SearchResultComponent } from './get-request-components/search-result/search-result.component';
import { authGuard } from './services/guards/auth.guard';
import { UserParticipationHistoryComponent } from './get-request-components/user-participation-history/user-participation-history.component';
import { ReviewsUserAddedComponent } from './get-request-components/reviews-user-added/reviews-user-added.component';
import { CommentsUserAddedComponent } from './get-request-components/comments-user-added/comments-user-added.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  {
    path: 'search-result/:searchTerm',
    component: SearchResultComponent,
    canActivate: [authGuard],
  },
  {
    path: 'profile/:userId',
    component: ProfileComponent,
    canActivate: [authGuard],
    canActivateChild: [authGuard],
    children: [
      {
        path: 'user-information',
        component: UserInfoComponent,
      },
      {
        path: 'user-analytics',
        component: UserAnalyticsComponent,
      },
      {
        path: 'participation-history',
        component: UserParticipationHistoryComponent,
      },
      { path: 'reviews', component: ReviewsUserAddedComponent },
      {
        path: 'comments',
        component: CommentsUserAddedComponent,
      },
    ],
  },
  {
    path: 'organizer-panel/:organizerId',
    component: OrganizerPanelComponent,
    canActivate: [authGuard],
  },
  {
    path: 'admin-panel',
    component: AdminPanelComponent,
    canActivate: [authGuard],
  },
  {
    path: 'available-promo-codes',
    component: AvailablePromoCodesComponent,
    canActivate: [authGuard],
  },
  { path: 'events', component: EventsComponent, canActivate: [authGuard] },
  {
    path: 'events/event/:eventId',
    component: EventPageComponent,
    canActivate: [authGuard],
  },
  {
    path: 'locations',
    component: LocationsComponent,
    canActivate: [authGuard],
  },
  {
    path: 'locations/location/:locationId',
    component: LocationPageComponent,
    canActivate: [authGuard],
  },
  {
    path: 'organizers',
    component: OrganizersComponent,
    canActivate: [authGuard],
  },
  {
    path: 'organizers/organizer/:organizerId',
    component: OrganizerPageComponent,
    canActivate: [authGuard],
  },
  { path: '**', component: NotFoundComponent },
];
