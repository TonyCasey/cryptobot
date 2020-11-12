import { BotService } from './../shared/services/bot.service';
import { ChartsModule } from './../charts/charts.module';
// Angular Imports
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

// This Module's Components
import { BotComponent } from './bot.component';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '@angular/material';
import { FormsModule } from '@angular/forms';
import { OrderService, PositionService, CoinService, ExchangeService,
    IndicatorService, RuleSetService, RuleService } from '../shared/services';

@NgModule({
    imports: [
        CommonModule,
        MaterialModule,
        FormsModule,
        ChartsModule
    ],
    declarations: [
        BotComponent
    ],
    exports: [
        BotComponent,
    ],
    providers: [
        BotService,
        OrderService,
        PositionService,
        CoinService,
        ExchangeService,
        IndicatorService,
        RuleSetService,
        RuleService
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class BotModule {}
