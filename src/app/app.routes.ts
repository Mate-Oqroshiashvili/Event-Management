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
import { UserInfoComponent } from './get-request-components/user/user-info/user-info.component';
import { UserAnalyticsComponent } from './get-request-components/user/user-analytics/user-analytics.component';
import { UserParticipationHistoryComponent } from './get-request-components/user/user-participation-history/user-participation-history.component';
import { ReviewsUserAddedComponent } from './get-request-components/user/reviews-user-added/reviews-user-added.component';
import { CommentsUserAddedComponent } from './get-request-components/user/comments-user-added/comments-user-added.component';
import { OrganizerPanelComponent } from './get-request-components/organizer/organizer-panel/organizer-panel.component';
import { EventsComponent } from './get-request-components/event/events/events.component';
import { EventPageComponent } from './get-request-components/event/event-page/event-page.component';
import { LocationsComponent } from './get-request-components/location/locations/locations.component';
import { LocationPageComponent } from './get-request-components/location/location-page/location-page.component';
import { OrganizersComponent } from './get-request-components/organizer/organizers/organizers.component';
import { OrganizerPageComponent } from './get-request-components/organizer/organizer-page/organizer-page.component';
import { AddEventComponent } from './post-request-components/add-event/add-event.component';
import { AddLocationComponent } from './post-request-components/add-location/add-location.component';
import { AddOrganizerComponent } from './post-request-components/add-organizer/add-organizer.component';
import { AddArtistOnEventComponent } from './post-request-components/add-artist-on-event/add-artist-on-event.component';
import { AddSpeakerOnEventComponent } from './post-request-components/add-speaker-on-event/add-speaker-on-event.component';
import { registerGuard } from './services/guards/register.guard';
import { RescheduleEventComponent } from './put-request-components/reschedule-event/reschedule-event.component';
import { TicketModalComponent } from './modals/ticket-modal/ticket-modal.component';
import { AddTicketComponent } from './post-request-components/add-ticket/add-ticket.component';
import { ActiveTicketsComponent } from './get-request-components/user/active-tickets/active-tickets.component';
import { adminGuard } from './services/guards/admin.guard';
import { organizerGuard } from './services/guards/organizer.guard';
import { CreatePromoCodeComponent } from './post-request-components/create-promo-code/create-promo-code.component';
import { promoGuard } from './services/guards/promo.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent, canDeactivate: [promoGuard] },
  {
    path: 'register',
    component: RegisterComponent,
    canDeactivate: [registerGuard],
  },
  { path: 'login', component: LoginComponent },
  {
    path: 'search-result/:searchTerm',
    component: SearchResultComponent,
    canActivate: [authGuard],
  },
  {
    path: 'profile/:userId',
    component: UserInfoComponent,
    canActivate: [authGuard],
    canActivateChild: [authGuard],
    children: [
      {
        path: 'user-analytics',
        component: UserAnalyticsComponent,
        outlet: 'bottom',
      },
      {
        path: 'active-tickets',
        component: ActiveTicketsComponent,
        outlet: 'bottom',
      },
      {
        path: 'participation-history',
        component: UserParticipationHistoryComponent,
        outlet: 'bottom',
      },
      {
        path: 'reviews',
        component: ReviewsUserAddedComponent,
        outlet: 'bottom',
      },
      {
        path: 'comments',
        component: CommentsUserAddedComponent,
        outlet: 'bottom',
      },
      {
        path: '',
        redirectTo: 'user-analytics',
        pathMatch: 'full',
        outlet: 'bottom',
      },
    ],
  },
  {
    path: 'profile/:userId/update-user',
    component: UpdateUserComponent,
    canActivate: [authGuard],
  },
  {
    path: 'profile/:userId/register-as-organizer',
    component: AddOrganizerComponent,
    canActivate: [authGuard],
  },
  {
    path: 'organizer-panel/:organizerId',
    component: OrganizerPanelComponent,
    canActivate: [authGuard, organizerGuard],
    canActivateChild: [authGuard, organizerGuard],
    children: [
      {
        path: 'add-artist-on-event/:eventId',
        outlet: 'add-modal',
        component: AddArtistOnEventComponent,
      },
      {
        path: 'add-speaker-on-event/:eventId',
        outlet: 'add-modal',
        component: AddSpeakerOnEventComponent,
      },
    ],
  },
  {
    path: 'organizer-panel/:organizerId/update-organizer',
    component: UpdateOrganizerComponent,
    canActivate: [authGuard, organizerGuard],
  },
  {
    path: 'organizer-panel/:organizerId/:eventId/add-ticket',
    component: AddTicketComponent,
    canActivate: [authGuard, organizerGuard],
  },
  {
    path: 'organizer-panel/:organizerId/reschedule-event/:eventId',
    component: RescheduleEventComponent,
    canActivate: [authGuard, organizerGuard],
  },
  {
    path: 'organizer-panel/:organizerId/update-promo-code/:promoCodeId',
    component: UpdatePromoCodeComponent,
    canActivate: [authGuard, organizerGuard],
  },
  {
    path: 'admin-panel',
    component: AdminPanelComponent,
    canActivate: [authGuard, adminGuard],
  },
  {
    path: 'admin-panel/add-location',
    component: AddLocationComponent,
    canActivate: [authGuard, adminGuard],
  },
  { path: 'events', component: EventsComponent, canActivate: [authGuard] },
  {
    path: 'events/add-event',
    component: AddEventComponent,
    canActivate: [authGuard, organizerGuard],
  },
  {
    path: 'events/event/:eventId',
    component: EventPageComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'ticket-page/:ticketId',
        component: TicketModalComponent,
        outlet: 'modal',
      },
    ],
  },
  {
    path: 'events/event/:eventId/create-promo-code',
    component: CreatePromoCodeComponent,
    canActivate: [authGuard, organizerGuard],
  },
  {
    path: 'events/event/:eventId/update-event',
    component: UpdateEventComponent,
    canActivate: [authGuard, organizerGuard],
  },
  {
    path: 'events/event/:eventId/update-event-images',
    component: UpdateImagesComponent,
    canActivate: [authGuard, organizerGuard],
  },
  {
    path: 'events/event/:eventId/update-ticket/:ticketId',
    component: UpdateTicketComponent,
    canActivate: [authGuard, organizerGuard],
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
    path: 'locations/location/:locationId/update-location',
    component: UpdateLocationComponent,
    canActivate: [authGuard, adminGuard],
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
