import { Component, OnInit } from '@angular/core';
import { EventDto, EventService } from '../../services/event/event.service';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-search-result',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './search-result.component.html',
  styleUrl: './search-result.component.css',
})
export class SearchResultComponent implements OnInit {
  searchTerm: string = '';
  events: EventDto[] = [];

  constructor(
    private eventService: EventService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((data) => {
      const seachTerm = data.get('searchTerm');
      if (seachTerm) {
        this.searchTerm = seachTerm;
        this.getFoundEvents();
      } else {
        console.error('Search term not found in route parameters');
      }
    });
  }

  getFoundEvents() {
    this.eventService.getEventsBySearchTerm(this.searchTerm).subscribe({
      next: (data: any) => {
        this.events = data.events;
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Fetched the events by search term successfully!');
      },
    });
  }

  searchLogic(searchTerm: string) {
    this.router.navigate(['/search-result', searchTerm]);
    searchTerm = '';
  }
}
