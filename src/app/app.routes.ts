import { Routes } from '@angular/router';
import { HomeComponent } from './get-request-components/home/home.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { AdminPanelComponent } from './get-request-components/admin-panel/admin-panel.component';
import { RegisterComponent } from './post-request-components/register/register.component';
import { LoginComponent } from './post-request-components/login/login.component';
import { authGuard } from './services/guards/auth.guard';
import { UpdateImagesComponent } from './put-request-components/update-images/update-images.component';
import { UpdateEventComponent } from './put-request-components/update-event/update-event.component';
import { UpdateLocationComponent } from './put-request-components/update-location/update-location.component';
import { UpdateOrganizerComponent } from './put-request-components/update-organizer/update-organizer.component';
import { UpdatePromoCodeComponent } from './put-request-components/update-promo-code/update-promo-code.component';
import { UpdateTicketComponent } from './put-request-components/update-ticket/update-ticket.component';
import { UpdateUserComponent } from './put-request-components/update-user/update-user.component';
import { SearchResultComponent } from './get-request-components/event/search-result/search-result.component';
import { ProfileComponent } from './get-request-components/user/profile/profile.component';
import { UserInfoComponent } from './get-request-components/user/user-info/user-info.component';
import { UserAnalyticsComponent } from './get-request-components/user/user-analytics/user-analytics.component';
import { UserParticipationHistoryComponent } from './get-request-components/user/user-participation-history/user-participation-history.component';
import { ReviewsUserAddedComponent } from './get-request-components/user/reviews-user-added/reviews-user-added.component';
import { CommentsUserAddedComponent } from './get-request-components/user/comments-user-added/comments-user-added.component';
import { OrganizerPanelComponent } from './get-request-components/organizer/organizer-panel/organizer-panel.component';
import { AvailablePromoCodesComponent } from './get-request-components/event/available-promo-codes/available-promo-codes.component';
import { EventsComponent } from './get-request-components/event/events/events.component';
import { EventPageComponent } from './get-request-components/event/event-page/event-page.component';
import { LocationsComponent } from './get-request-components/location/locations/locations.component';
import { LocationPageComponent } from './get-request-components/location/location-page/location-page.component';
import { OrganizersComponent } from './get-request-components/organizer/organizers/organizers.component';
import { OrganizerPageComponent } from './get-request-components/organizer/organizer-page/organizer-page.component';
import { PromoCodesComponent } from './get-request-components/organizer/promo-codes/promo-codes.component';
import { TicketsComponent } from './get-request-components/organizer/tickets/tickets.component';
import { AddEventComponent } from './post-request-components/add-event/add-event.component';
import { AddLocationComponent } from './post-request-components/add-location/add-location.component';
import { AddOrganizerComponent } from './post-request-components/add-organizer/add-organizer.component';
import { AddOrganizerOnLocationComponent } from './post-request-components/add-organizer-on-location/add-organizer-on-location.component';

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
        path: 'register-as-organizer',
        component: AddOrganizerComponent,
      },
      {
        path: 'user-information',
        component: UserInfoComponent,
      },
      {
        path: 'user-information/update-user',
        component: UpdateUserComponent,
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
    path: 'events/add-event',
    component: AddEventComponent,
    canActivate: [authGuard],
  },
  {
    path: 'events/event/:eventId',
    component: EventPageComponent,
    canActivate: [authGuard],
  },
  {
    path: 'events/event/:eventId/update-event',
    component: UpdateEventComponent,
    canActivate: [authGuard],
  },
  {
    path: 'events/event/:eventId/update-event-images',
    component: UpdateImagesComponent,
    canActivate: [authGuard],
  },
  {
    path: 'locations',
    component: LocationsComponent,
    canActivate: [authGuard],
  },
  {
    path: 'locations/add-location',
    component: AddLocationComponent,
    canActivate: [authGuard],
  },
  {
    path: 'locations/location/:locationId',
    component: LocationPageComponent,
    canActivate: [authGuard],
  },
  {
    path: 'locations/location/:locationId/update-location',
    component: UpdateLocationComponent,
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
  {
    path: 'organizers/organizer/:organizerId/add-organizer-on-specific-location',
    component: AddOrganizerOnLocationComponent,
    canActivate: [authGuard],
  },
  {
    path: 'organizers/organizer/:organizerId/update-organizer',
    component: UpdateOrganizerComponent,
    canActivate: [authGuard],
  },
  {
    path: 'organizers/organizer/:organizerId/promo-codes',
    component: PromoCodesComponent,
    canActivate: [authGuard],
  },
  {
    path: 'organizers/organizer/:organizerId/promo-codes/update-promo-code/:promoCodeId',
    component: UpdatePromoCodeComponent,
    canActivate: [authGuard],
  },
  {
    path: 'organizers/organizer/:organizerId/tickets',
    component: TicketsComponent,
    canActivate: [authGuard],
  },
  {
    path: 'organizers/organizer/:organizerId/tickets/update-ticket/:ticketId',
    component: UpdateTicketComponent,
    canActivate: [authGuard],
  },
  { path: '**', component: NotFoundComponent },
];
