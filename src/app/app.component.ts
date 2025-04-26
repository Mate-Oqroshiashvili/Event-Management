import { Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NavbarComponent } from './navbar/navbar.component';
import { FooterComponent } from './footer/footer.component';
import { EventSocketService } from './services/web sockets/event-socket.service';
import { isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [NavbarComponent, RouterModule, FooterComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  publishedEventTitle: string[] = [];
  updatedEventTitle: string[] = [];
  rescheduledEventTitle: string[] = [];
  deletedEventTitle: string[] = [];

  constructor(
    private eventSocket: EventSocketService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.eventSocket.startConnection();

      this.eventSocket.publishedEventTitle$.subscribe((data: string | null) => {
        if (data) {
          this.publishedEventTitle.push(data);
          const index = this.publishedEventTitle.length - 1;

          setTimeout(() => {
            this.publishedEventTitle.splice(index, 1);
          }, 30000);
        }
      });

      this.eventSocket.updatedEventTitle$.subscribe((data: string | null) => {
        if (data) {
          this.updatedEventTitle.push(data);
          const index = this.updatedEventTitle.length - 1;

          setTimeout(() => {
            this.updatedEventTitle.splice(index, 1);
          }, 30000);
        }
      });

      this.eventSocket.rescheduledEventTitle$.subscribe(
        (data: string | null) => {
          if (data) {
            this.rescheduledEventTitle.push(data);
            const index = this.rescheduledEventTitle.length - 1;

            setTimeout(() => {
              this.rescheduledEventTitle.splice(index, 1);
            }, 30000);
          }
        }
      );

      this.eventSocket.deletedEventTitle$.subscribe((data: string | null) => {
        if (data) {
          this.deletedEventTitle.push(data);
          const index = this.deletedEventTitle.length - 1;

          setTimeout(() => {
            this.deletedEventTitle.splice(index, 1);
          }, 30000);
        }
      });
    }
  }

  removeCreatedToast(index: number): void {
    this.publishedEventTitle.splice(index, 1);
  }

  removeUpdatedToast(index: number): void {
    this.updatedEventTitle.splice(index, 1);
  }

  removeRescheduledToast(index: number): void {
    this.rescheduledEventTitle.splice(index, 1);
  }

  removeDeletedToast(index: number): void {
    this.deletedEventTitle.splice(index, 1);
  }
}
