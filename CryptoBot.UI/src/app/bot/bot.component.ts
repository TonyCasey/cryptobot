import { BotService, OrderService, IndicatorService, PositionService, RuleService,
    RuleSetService, ExchangeService, CoinService } from './../shared/services';
import { BotResponseDto, BotSearchResponseDto, PositionResponseDto,
    OrderResponseDto, IndicatorResponseDto, RuleSetResponseDto, RuleResponseDto,
    ExchangeResponseDto, CoinResponseDto, PositionRequestDto, OrderRequestDto } from './../shared/model/dto/models';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { OnInit } from '@angular/core/src/metadata/lifecycle_hooks';
import { ActivatedRoute } from '@angular/router';
import { APPCONFIG } from '../config';
@Component({
    selector: 'bot',
    templateUrl: 'bot.component.html',
    styleUrls: ['bot.component.scss']
})
export class BotComponent implements OnInit {

    AppConfig: any;
    errorMessage: any;
    selectedExchange: number;
    bot = new BotResponseDto();
    coin: CoinResponseDto;
    baseCoin: CoinResponseDto;
    exchange: ExchangeResponseDto;
    symbolPair = 'BTCEUR';
    orders: OrderResponseDto[];
    positions: PositionResponseDto[];
    currentPosition: PositionResponseDto;
    decimalFormat: string;
    indicators: IndicatorResponseDto[];
    totalProfit = 0;
    totalProfitPercent = 0;
    constructor(private _router: Router,
        private _activeRoute: ActivatedRoute,
        private _botService: BotService,
        private _orderService: OrderService,
        private _positionService: PositionService,
        private _coinService: CoinService,
        private _exchangeService: ExchangeService,
        private _indicatorService: IndicatorService,
        private _ruleSetService: RuleSetService,
        private _ruleService: RuleService
    ) {

    }


    ngOnInit() {

        this.AppConfig = APPCONFIG;

        this._activeRoute.params.subscribe((queryParams) => {

            const Id = +queryParams['Id'];

            if( Id !== null) {
                this._botService
                .get(Id)
                .subscribe((response: BotSearchResponseDto) => {
                    this.bot = response;
                    this.symbolPair = 'BTCEUR'
                    },
                    error => this.errorMessage = <any> error
                );
            }
        });
      }

      getBot(id: number) {
        this._botService.get(id)
        .subscribe((result) => {

          this.bot = result;
          this.coin = this.getCoin(result.coinId);
          this.decimalFormat  = '1.2-' + this.coin.orderRoundingExponent;
          this.baseCoin = this.getCoin(result.baseCoinId);
          this.exchange = this.getExchange(result.exchangeId);

          this.getOrders();

        },
          error => this.errorMessage = <any> error
        );
      }

      getCoin(id: number): CoinResponseDto {
        this._coinService.get(id)
        .subscribe((result) => {
          return result;
        },
          error => this.errorMessage = <any> error
        );
        return null;
      }

      getExchange(id: number): ExchangeResponseDto {
        this._exchangeService.get(id)
        .subscribe((result) => {
          return result;
        },
          error => this.errorMessage = <any> error
        );
        return null;
      }

        getOrders() {
        this._orderService.search(new OrderRequestDto({botId: this.bot.botId}))
        .subscribe((result) => {
          this.orders = result.data;
          this.getPositions();
        },
          error => this.errorMessage = <any> error
        );
      }

      getPositions() {
        this._positionService.search(new PositionRequestDto({botId: this.bot.botId}))
        .subscribe((result) => {
          this.positions = result.data
          this.currentPosition = result.data.find(x => x.positionId === this.bot.currentPositionId);
          result.data.forEach(position => {
            this.totalProfit += position.netProfit || 0;
          });
          this.getNetProfitPercent();
        },
          error => this.errorMessage = <any> error
        )
        ;
      }
      tabChanged(tabChangeEvent: any) {
        switch(tabChangeEvent.index) {
          case 0 : // summary
          break;
          case 1 : // positions
            this.getPositions();
          break;
          case 2 : // settings
          break;
          case 3 : // indicators
          break;
          case 4 : // back testing
          break;
        }
      }

      getNetProfitPercent() {
        // total profit as a percentage of first trade cost
        const firstTradeCost = this.positions[0].buyPrice * this.positions[0].quantity;
        this.totalProfitPercent = (this.totalProfit * 100) / firstTradeCost;
      }
}
